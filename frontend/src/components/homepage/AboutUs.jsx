import React from 'react';
import './AboutUs.css';
import image5 from '../../assets/images/image5.jpg';

const AboutUs = ({ onMenuClick, onDreamCupClick }) => {
    return (
        <div className="about-section">
            <img src={image5} alt="Café" className="about-image" />
            <div className="about-description">
                <h2>OM OSS</h2>
                <p>
                    Vi er <i>Matcha og Mocha</i>, en koselig café med gode smaker og kvalitet i fokus.<br />
                    Vår drøm er å skape en uforglemmelig opplevelse for våre gjester gjennom "Drømmekoppen".
                </p>
                <br />
                <p>
                    Vi er stolte av å tilby eksepsjonell service, sikre at hver slurk av drikken vår er deilig.<br />
                    Vi ønsker å være en del av din hverdag, og å være en sted hvor du ser frem til å drikkke den asiatiske grønne drikken som inneholder koffein!<br />
                    Men derimot er dette fine grønne pulveret er kjent for å være sunnere alternativ for kaffe da den i tillegg har en beroligende effekt, slik energi innskuddet ikke virker drastisk, men kan dras bedre nytte av over lengre tid..
                </p>
                <br />
                <p>
                    Frist smaksløkene med vår meny – eller design din egen kopp i "Drømmekoppen!".
                </p>
                <div className="about-buttons">
                    <button className="navbar-button" onClick={onMenuClick}>
                        Se Meny
                    </button>
                    <button className="navbar-button" onClick={onDreamCupClick}>
                        Lag din egen kopp
                    </button>
                </div>
            </div>

        </div>
    );
};

export default AboutUs;