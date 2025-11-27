import React from 'react';
import './InfoBox.css';
import { SERVER_URL } from '../../shared/apiConfig'; // Import SERVER_URL

const InfoBox = ({ error, selectedIngredient, addSelectedIngredient, cupTheme }) => {
    return (
        <div
            className="box info"

        >
            <h2>{selectedIngredient ? selectedIngredient.name : "Info"}</h2>
            {error && <p style={{ color: "red" }}>{error}</p>}
            {selectedIngredient ? (
                <>
                    <div className="info-image-container">
                        <img
                            src={`${SERVER_URL}${selectedIngredient.imagePath}`}
                            alt={selectedIngredient.name}
                            className="ingredient-image-box"
                            onError={(e) => (e.target.src = '/images/default.png')}
                        />
                    </div>
                    <p className="info-description">
                        {selectedIngredient.description.split('\n').map((line, idx) => (
                            <span key={idx}>
                                {idx === 0 ? (
                                    <span className="highlight">{line}</span>
                                ) : (
                                    line
                                )}
                                <br />
                            </span>
                        ))}
                    </p>
                </>
            ) : (
                <p className="filler-text">Velg en ingrediens for å se informasjon.</p>
            )}
            {selectedIngredient && (
                <div className="add-button-container">
                    <button className="add-button" onClick={() => addSelectedIngredient(selectedIngredient)}>
                        Legg til
                    </button>
                </div>
            )}
        </div>
    );
};

export default InfoBox;