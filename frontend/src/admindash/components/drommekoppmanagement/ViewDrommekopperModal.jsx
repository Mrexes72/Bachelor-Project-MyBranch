import React from 'react';
import './DrommekopperModal.css';

const ViewDrommekopperModal = ({ isOpen, onClose, drink }) => {
    if (!isOpen) return null;

    return (
        <div className="modal">
            <div className="modal-content">
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>Drink Details</h2>
                <p><strong>Name:</strong> {drink.name}</p>
                <p><strong>Category:</strong> {drink.categoryName}</p>
                <p><strong>Price (NOK):</strong> {drink.salePrice}</p>
                <p><strong>Favorites:</strong> {drink.timesFavorite}</p>
                <p><strong>Ingredients:</strong> {drink.ingredientDTOs.map(ingredient => ingredient.name).join(', ')}</p>
            </div>
        </div>
    );
};

export default ViewDrommekopperModal;