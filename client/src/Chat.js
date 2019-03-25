import React, { Component } from 'react';

class Chat extends Component {

    constructor(props) {
        super(props);

        this.state = {
            message: '',
        };
        this.sendMessage = this.sendMessage.bind(this)
        this.renderSubmit = this.renderSubmit.bind(this)
    }

    sendMessage = (e) => {
    e.preventDefault();
    this.props.hubConnection
        .invoke('sendMessageToHub', this.props.userName, this.state.message)
        .catch(err => console.error(err));
        this.setState({message: ''});      
    };

   
    componentDidUpdate(){
        this.scrollToBottom();
      }
      
      scrollToBottom() {
        const {history} = this.refs;
        history.scrollTop = history.scrollHeight - history.clientHeight;
      }


    displayMessage(message, i){
        return (
            <div key={i}> 
                <div className="chat-message ">
                    <div className="chat-message-content" >
                        <h5 style={{color:message.color}}>{message.userName}</h5>
                        <p>{message.message}</p>
                    </div> 
                </div> 
                <hr/>
            </div>
        )
    }

    renderSubmit(){
        return (
            <form>
                <div className="form-row" >
                    <div className=" form-group col-md-10">
                        <input type='text'
                            className="form-control"
                            onChange={e => this.setState({ message: e.target.value })}
                            value={this.state.message}
                            placeholder='Your message'/>
                    </div>
                    <div className="form-group col-md-2">
                        <button 
                            type="submit"
                            onClick={this.sendMessage}
                            className="btn btn-primary">
                            Send
                        </button>
                    </div>
                </div>
            </form>
        )
    }
    render(){
        const {userName, messages} = this.props
        return (
            <div id="live-chat">
                <header>
                    <h4>{userName}</h4>
                </header>
                <div className="chat">
                    <div className="chat-history" ref={`history`}>
                        {messages.map((message, index) => this.displayMessage(message, index))}
                    </div> 
                    {this.renderSubmit()}
                </div> 
            </div> 
        )
    }
}

export default Chat;
