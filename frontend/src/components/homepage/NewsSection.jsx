import React from 'react';

const NewsSection = ({ onMenuClick, onDreamCupClick }) => {
    return (
        <div className="news-section">
            <div className="new-item">
                <h2>Nyheter</h2>
                <p>Matcha og Mocha er en koselig café med fokus på gode smaker og kvalitet.<br />
                    Vi brenner for å skape en avslappende pause i hverdagen for alle som er glad i drikkeopplevelser utenom det vanlige!
                </p>
                <div className="menu-button">
                    <button className="navbar-button" onClick={onMenuClick}>
                        Se Meny
                    </button>
                </div>
            </div>

            <div className="new-item">
                <h2>Drømmekoppen</h2>
                <p>Lag din egen favorittdrikk med <i>Drømmekoppen!</i><br />
                    Velg ingredienser i sanntid og se koppen fylle seg opp. Gled deg til en unik interaktiv opplevelse.
                </p>
                <div className="dream-cup-button">
                    <button className="navbar-button" onClick={onDreamCupClick}>
                        Lag din egen kopp
                    </button>
                </div>
            </div>

        </div>
    );
};

export default NewsSection;