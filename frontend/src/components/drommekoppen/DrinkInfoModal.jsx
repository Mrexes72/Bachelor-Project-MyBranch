import React, { useState, useEffect } from 'react';
import './DrinkInfoModal.css';
import DreamCup2dSeperateFill from '../animasjoner/DreamCup2dSeperateFill';

const DrinkInfoModal = ({ isOpen, onClose, onSave, initialDrinkName, isCreating, cupTheme, setCupTheme, layers, setLayers, selectedIngredients }) => {
    const [drinkName, setDrinkName] = useState(initialDrinkName);

    useEffect(() => {
        setDrinkName(initialDrinkName);
    }, [initialDrinkName]);

    if (!isOpen) return null;

    const handleSave = () => {
        if (isCreating) return; // Prevent multiple calls while saving
        onSave(drinkName); // Save the drink name
        onClose(); // Close the modal
    };

    // Format ingredients into groups of three
    const formatIngredients = (ingredients) => {
        const groups = [];
        for (let i = 0; i < ingredients.length; i += 3) {
            groups.push(ingredients.slice(i, i + 3).map(ing => ing.name).join(', '));
        }
        return groups;
    };

    return (
        <div className="modal">
            <div className="modal-content">
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>Lag din drømmekopp</h2>
                <input
                    type="text"
                    value={drinkName}
                    onChange={(e) => setDrinkName(e.target.value)}
                    placeholder="Skriv inn navn på drikken"
                    className="drink-name-input"
                />
                <p className="modal-description">
                    Skriv inn et navn for din drømmekopp. Trykk "Lagre" for å bekrefte eller "Avbryt" for å gå tilbake.
                </p>
                {/* List of selected ingredients */}
                <div className="selected-ingredients-list">
                    <h3>Valgte ingredienser:</h3>
                    {selectedIngredients.length > 0 ? (
                        <div className="ingredients-text">
                            {formatIngredients(selectedIngredients).map((group, index) => (
                                <p key={index}>{group}</p>
                            ))}
                        </div>
                    ) : (
                        <p>Ingen ingredienser valgt.</p>
                    )}
                </div>

                <div className="cup-preview">
                    <DreamCup2dSeperateFill
                        cupTheme={cupTheme}
                        layers={layers}
                        setLayers={setLayers}
                    />

                    {/* Cup theme buttons */}
                    <div style={{ display: 'flex', gap: '10px', marginTop: '10px' }}>
                        {[
                            { lid: "#A1C48E", cup: "#7FA87F", straw: "#5E8B5E" }, // Matcha theme
                            { lid: "#9ABAD9", cup: "#6C96BA", straw: "#3E6587" }, // Espresso theme
                            { lid: "#FFE59A", cup: "#FFC857", straw: "#FFB000" }, // Smoothie theme
                            { lid: "#DDB0D4", cup: "#B981BD", straw: "#9A5A9F" }, // Berry theme
                            { lid: "#3E4C59", cup: "#627C8C", straw: "#AAB4C0" }, // Stormy Sea theme
                            { lid: "#A42CD6", cup: "#D94CF6", straw: "#F0AAFF" }, // Neon Purple theme
                            { lid: "#FF6347", cup: "#FF8566", straw: "#FFA488" }, // Sunset Orange theme
                            { lid: "#2E8B57", cup: "#3CB371", straw: "#66CDAA" }  // Forest Green theme
                        ].map((theme, index) => (
                            <button
                                key={index}
                                onClick={() => setCupTheme(theme)}
                                style={{
                                    backgroundColor: theme.cup,
                                    border: '2px solid #000',
                                    borderRadius: '50%',
                                    width: '30px',
                                    height: '30px',
                                    cursor: 'pointer',
                                }}
                            />
                        ))}
                    </div>
                </div>

                <div className="modal-buttons">
                    <button
                        className="save-button"
                        onClick={handleSave}
                        disabled={isCreating}
                    >
                        {isCreating ? "Lagrer..." : "Lagre"}
                    </button>
                    <button className="cancel-button" onClick={onClose}>Avbryt</button>
                </div>
            </div>
        </div>
    );
};

export default DrinkInfoModal;