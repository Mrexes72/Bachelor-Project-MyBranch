import React from 'react';
import Navbar from '../shared/Navbar';
import Footer from '../shared/Footer';
import Drommekopp from '../components/drommekoppen/Drommekopp';
import './Drommekoppen.css';

const Drommekoppen = () => {
    return (
        <div className='page-container'>
            <Navbar />
            <div className="drommekopp-content">
                <Drommekopp />
            </div>
            <Footer />
        </div>
    );
};

export default Drommekoppen;