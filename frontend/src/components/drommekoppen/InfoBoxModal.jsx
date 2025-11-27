import React from 'react';
import './InfoBoxModal.css';

const InfoBoxModal = ({ isOpen, onClose, ingredient, addSelectedIngredient }) => {
    if (!isOpen) return null;

    return (
        <div className="modal">
            <div className="modal-content">
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>{ingredient.name}</h2>
                <img src={ingredient.image} alt={ingredient.name} className="ingredient-image" />
                <p>{ingredient.description}</p>
                <button
                    className="add-button"
                    onClick={() => {
                        if (ingredient) {
                            addSelectedIngredient(ingredient); // Pass the ingredient explicitly
                            onClose(); // Close the modal
                        } else {
                            console.warn("No ingredient provided to add.");
                        }
                    }}
                >
                    Add
                </button>
            </div>
        </div>
    );
};

export default InfoBoxModal;