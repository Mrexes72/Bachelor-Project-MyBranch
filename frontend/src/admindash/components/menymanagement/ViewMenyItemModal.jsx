import React from 'react';
import './MenyItemModal.css';

const ViewMenuItemModal = ({ isOpen, onClose, menuItem, handleEdit, handleDelete }) => {
    if (!isOpen) return null;

    return (
        <div className="modal">
            <div className="modal-content">
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>Menu Item Details</h2>
                <p><strong>Name:</strong> {menuItem.name}</p>
                <p><strong>Category:</strong> {menuItem.categoryName}</p>
                <p><strong>Description:</strong> {menuItem.description}</p>
                <p><strong>Price (NOK):</strong> {menuItem.price}</p>
                <p><strong>Available:</strong> {menuItem.isAvailable ? 'Yes' : 'No'}</p>
                <div className="menu-item-actions">
                    <button onClick={() => handleEdit(menuItem)} className="edit-button">
                        Edit
                    </button>
                    <button onClick={() => handleDelete(menuItem.menuItemId)} className="delete-button">
                        Delete
                    </button>
                    <button className="cancel-button" onClick={onClose}>Cancel</button>
                </div>
            </div>
        </div>
    );
};

export default ViewMenuItemModal;