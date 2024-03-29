import * as React from 'react';
import NavMenu from './NavMenu';


class Layout extends React.Component {
     render() {
        return <div>
        <NavMenu />
        <div className='container-fluid'>
            {this.props.children}
        </div>
    </div>;
    }
}

export default Layout
