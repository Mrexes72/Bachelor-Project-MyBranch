import React from 'react';
import './MenyItemModal.css';

const AddMenuItemModal = ({ isOpen, onClose, onSubmit, menuItem, handleInputChange, categories }) => {
    if (!isOpen) return null;

    return (
        <div className="modal">
            <div className="modal-content-menu-management">
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>Add New Menu Item</h2>
                <p className="modal-description">Enter the details for the new menu item</p>
                <form onSubmit={onSubmit} className="add-menu-item-form">
                    <div className="form-group">
                        <label htmlFor="name">Menu Item Name</label>
                        <p className="field-description">Enter the name of the menu item</p>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={menuItem.name}
                            onChange={handleInputChange}
                            placeholder="Enter menu item name"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="category">Category</label>
                        <p className="field-description">Select or enter the menu item category</p>
                        <select
                            id="category"
                            name="categoryId"
                            value={menuItem.categoryId}
                            onChange={handleInputChange}
                            required
                        >
                            <option value="">Select a category</option>
                            {categories.map(category => (
                                <option key={category.categoryId} value={category.categoryId}>
                                    {category.name}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="form-group">
                        <label htmlFor="description">Description</label>
                        <p className="field-description">Enter the description of the menu item</p>
                        <textarea
                            id="description"
                            name="description"
                            value={menuItem.description}
                            onChange={handleInputChange}
                            placeholder="Enter description"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="price">Price (NOK)</label>
                        <p className="field-description">Enter the price per unit in Norwegian Krone</p>
                        <input
                            type="number"
                            id="price"
                            name="price"
                            value={menuItem.price}
                            onChange={handleInputChange}
                            placeholder="Enter price"
                            min="0"
                            step="0.01"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="isAvailable">Available</label>
                        <input
                            type="checkbox"
                            id="isAvailable"
                            name="isAvailable"
                            checked={menuItem.isAvailable}
                            onChange={handleInputChange}
                        />
                    </div>
                    <div>
                        <div className='modal-actions'>
                            <button type="submit" className="add-button">Add Menu Item</button>
                            <button className="cancel-button-menu-management" onClick={onClose}>Cancel</button></div>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default AddMenuItemModal;