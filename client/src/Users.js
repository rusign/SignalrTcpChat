import React, { Component } from 'react';
import { SketchPicker } from 'react-color';
import Popup from "reactjs-popup";

var currentUserColor
class Users extends Component {

    constructor(props) {
        super(props);
        this.state= {
            open: false,
            currentUserName:"",
            currentUserColor:""
        }

        this.displayUser = this.displayUser.bind(this)
        this.handleColorChange = this.handleColorChange.bind(this)
        this.renderColorPicker = this.renderColorPicker.bind(this)
    }

    displayUser(user, i){
        return (
                <div key={i}className='row user-list-content '>
                    <div className='col-sm-6'> 
                        <p style={{fontSize:"16px"}}>{user.name}</p>
                    </div>
                    <div className='col-sm-3'>
                        <img  alt="" src={user.mute ? "mute.png" : "speaker.png" } onClick={() => this.props.muteFunc(user)} style={{cursor:"pointer",width:"16px", height:'16px'}}/>
                    </div>
                    <div className='col-sm-3'>
                    <button
                        style={{backgroundColor: user.color, width:"40px", height:"15px"}}
                        onClick={()=>{this.setState({currentUserName:user.name, open:!this.state.open})}} // change state.color in handleChange
                        />
                    </div>
                </div>
        )
    }

    handleColorChange(color, event)
    {
        currentUserColor = color.hex
    }

    renderColorPicker(){
        return (
            this.state.open?
            <Popup contentStyle={{width:"auto"}} open={this.state.open}
                    onClose={()=>{this.setState({open:!this.state.open}); this.props.handleUserColorChange(this.state.currentUserName, currentUserColor)}}
                    position="top center">
                  <SketchPicker onChange={ this.handleColorChange }/>
            </Popup>
            :
            ""
        )
    }

    render(){
        const {users} = this.props
        return (
            <div id="live-user">
                <header>
                    <h4>Users online</h4>
                </header>
                <div className="user">
                    <div className="user-history">
                    {users.map((user, i) =>
                            this.displayUser(user, i)
                        )}
                    </div> 
                </div> 
                {this.renderColorPicker()}
            </div> 
        )
    }
}

export default Users;
