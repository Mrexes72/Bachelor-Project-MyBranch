import React from 'react';
import { useNavigate } from 'react-router-dom';
import Navbar from '../shared/Navbar';
import Footer from '../shared/Footer';
import Banner from '../components/homepage/Banner';
import ImageCarousel from '../components/homepage/ImageCarousel';
import NewsSection from '../components/homepage/NewsSection';
import AboutUs from '../components/homepage/AboutUs';
import './HomePage.css';

const HomePage = () => {

    const navigate = useNavigate();

    const handleMenuClick = () => {
        navigate('/meny');
    };

    const handleDreamCupClick = () => {
        navigate('/drommekoppen');
    };



    return (
        <div className="homepage">
            <Navbar />
            <div className="banner"><Banner /></div>

            <ImageCarousel />

            <div className="news-section">
                <NewsSection
                    onMenuClick={handleMenuClick}
                    onDreamCupClick={handleDreamCupClick}
                />
            </div>

            <AboutUs
                onMenuClick={handleMenuClick}
                onDreamCupClick={handleDreamCupClick}
            />
            <Footer />

        </div>
    );
};

export default HomePage;