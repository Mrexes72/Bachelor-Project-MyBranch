import React from "react";
import Meny from '../components/menypage/Meny';
/* import Nyheter from '../components/menypage/Nyheter'; */
import './MenyPage.css'
import Navbar from "../shared/Navbar";
import Footer from "../shared/Footer";

const MenyPage = () => {
    return (
        <div className="meny-page-container">
            <Navbar />
            <h2 className="meny-header">Menu List</h2>
            <div className="meny-page">
                <Meny />
                {/* <Nyheter /> */}
            </div>
            <Footer />
        </div>
    );
};

export default MenyPage;
