import * as React from 'react';

 class NavMenu extends React.Component {
     render() {
        return (
        <div style={{marginBottom: "3%"}}>
            <nav className="navbar navbar-light bg-dark">
            <a className="navbar-brand" style={{color:"white"}}>
                Chat
            </a>
        </nav>
      </div>)
      
    }
}

export default NavMenu