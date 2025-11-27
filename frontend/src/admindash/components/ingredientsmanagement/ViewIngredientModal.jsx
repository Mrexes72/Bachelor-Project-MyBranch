import React from 'react';
import './AddIngredientModal.css';

const ViewIngredientModal = ({ isOpen, onClose, ingredient, handleEdit, handleDelete }) => {
    if (!isOpen) return null;

    return (
        <div className="modal">
            <div className="modal-content-ingredient-management">
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>Ingredient Details</h2>
                <p><strong>Name:</strong> {ingredient.name}</p>
                <p><strong>Category:</strong> {ingredient.categoryName}</p>
                <p><strong>Price (NOK):</strong> {ingredient.unitPrice}</p>
                <p><strong>Available:</strong> {ingredient.isAvailable ? 'Yes' : 'No'}</p>
                <div className="ingredient-actions">
                    <button onClick={() => handleEdit(ingredient)} className="edit-button">
                        Edit
                    </button>
                    <button onClick={() => handleDelete(ingredient.ingredientId)} className="delete-button">
                        Delete
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ViewIngredientModal;