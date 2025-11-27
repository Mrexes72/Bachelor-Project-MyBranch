import React from 'react';
import './Footer.css';
import SocialLinks from '../components/homepage/SocialLinks';

const Footer = () => {

    return (
        <footer className="footer">

            <div className="footer-info">
                <div className="footer-section">
                    <h4>Åpningstider</h4>
                    <p>
                        Mandag - Fredag: 09:00 - 18:00 <br />
                        Lørdag: 10:00 - 16:00 <br />
                        Søndag: Stengt
                    </p>
                </div>
                <div className="footer-section">
                    <h4>Adresse</h4>
                    <p>TBA</p>
                </div>
                <div className="footer-section">
                    <h4>Følg oss</h4>
                    <SocialLinks />
                </div>
            </div>
            <div className="footer-bottom">
                <p>&copy; {new Date().getFullYear()} Matcha og Mocha. Alle rettigheter forbeholdt.</p>
            </div>
        </footer>
    );
};

export default Footer;