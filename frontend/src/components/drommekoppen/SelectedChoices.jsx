import React from 'react';
import './SelectedChoices.css';

const SelectedChoices = ({ selectedIngredients, removeIngredient, cupTheme }) => {
    return (
        <div className="box valg">
            <h2>Dine valg</h2>
            {selectedIngredients.length > 0 ? (
                <div className="selected-ingredients-grid">
                    {selectedIngredients.map((ingredient, index) => {
                        const backgroundColor = ingredient.color + '80';

                        return (
                            <div
                                key={index}
                                className="ingredient-item"
                                style={{
                                    borderColor: ingredient.color,
                                    backgroundColor: backgroundColor, // Set background color with 50% opacity
                                }}
                            >
                                <span>{ingredient.name}</span>
                                <button
                                    className="remove-button"
                                    onClick={() => removeIngredient(index)} // Trigger removal
                                >
                                    x
                                </button>
                            </div>
                        );
                    })}
                </div>
            ) : (
                <p className="filler-text">Ingen valgt</p>
            )}
        </div>
    );
};

export default SelectedChoices;