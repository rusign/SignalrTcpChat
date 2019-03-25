import React, { Component } from 'react';
import Layout from './Layout';
import Chat from './Chat'
import Users from './Users'
import './App.css'

const signalR = require("@aspnet/signalr");

class App extends Component {

  constructor(props) {
    super(props);

    this.state = {
      userName: '',
      message: '',
      messages: [],
      users: [],
      hubConnection: null,
      isCo :false,
      coLoading:true
    };
    this.muteFunction = this.muteFunction.bind(this)
    this.handleUserColorChange = this.handleUserColorChange.bind(this)
    this.getUserColor = this.getUserColor.bind(this)
  }

  muteFunction(user){
    this.state.users.some(e => {
        if(e.name === user.name)
          e.mute = !e.mute
        return null;
        })
   this.setState(this.state)
  }

  handleUserColorChange(name, color){
    this.state.users.some(e => {
      if(e.name === name)
         e.color = color
      return null; 
      })
    this.state.messages.some(e => {
        if(e.userName === name)
           e.color = color
        return null;
        })
    this.setState(this.state)
  }

  getUserColor(name){
    var color ="";
    this.state.users.some(e => {
      if(e.name === name)
      {
        color =  e.color
      }
      return null;
    })
    return color
  }

  componentDidMount = () => {
    const userName = window.prompt('Your name:', '');
    this.state.users.push({ name: userName ,mute: false, color:'#293239'})
    const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/chatter")
    .build();

    this.setState({ hubConnection, userName }, () => {
      this.state.hubConnection
        .start()
        .then(() => 
        {
          console.log('Connection')
          this.setState( {isCo:true,coLoading:false} );
          this.state.hubConnection.invoke('JoinHub', userName).catch(err => console.error(err));

          this.state.hubConnection.on('sendMessageToHub', (userName, receivedMessage) => {
            if(!this.state.users.some(e => e.name === userName))
                this.state.users.push({name:userName, mute: false, color:'#293239' })
            if(!this.state.users.some(e => {if (e.name === userName) return e.mute; return null; })) {
              var userColor = this.getUserColor(userName)
              const messages = this.state.messages.concat([{userName: userName, color: userColor ,message: receivedMessage}]);
                this.setState({ messages });
            }
            this.setState( this.state );
          });
          
          this.state.hubConnection.on('userDecoFromHub', (user) =>{
            var userColor = this.getUserColor(user)
            const messages = this.state.messages.concat([{userName: user, color: userColor , message: 'has left the chat'}]);
                this.setState({ messages });
            this.setState({messages, users :
               this.state.users.filter((x) => x.name !== user)})
          })

          this.state.hubConnection.on('inHub', (user) =>{
            if(!this.state.users.some(e => e.name === user))
                this.state.users.push({name:user, mute: false, color:'#293239' })
            this.setState( this.state );
          })
        })
        .catch(err => 
          {
            console.log('Error', err)
            this.setState( {isCo:false, coLoading:false} );
          });
    });
  };

  renderApp(){
    return (
      this.state.coLoading ?
        <Layout>
          Connection au server
        </Layout>
      :
        this.state.isCo ?
          <Layout>
            <div className='row'>
              <div className='col-sm-3'>
                <Users users={this.state.users} muteFunc={this.muteFunction} handleUserColorChange={this.handleUserColorChange}/> 
              </div>
              <div className='col-sm-9'>
              < Chat userName={this.state.userName} messages={this.state.messages} hubConnection={this.state.hubConnection}/> 
              </div>
            </div>
          </Layout>
        :
          <Layout>
            Connection with the server has failed
          </Layout>
      
    )
  }

  render() {
    return (
        this.state.hubConnection ?  this.renderApp() : ""
    )
  }
}

export default App;
