import React from 'react';
import './AddIngredientModal.css';

const EditIngredientModal = ({ isOpen, onClose, onSubmit, ingredient, handleInputChange, handleFileChange, categories }) => {
    if (!isOpen) return null;

    return (
        <div className="modal">
            <div className="modal-content-ingredient-management">
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>Edit Ingredient</h2>
                <p className="modal-description">Update the ingredient details below</p>
                <form onSubmit={onSubmit} className="add-ingredient-form">
                    <div className="form-group">
                        <label htmlFor="name">Ingredient Name</label>
                        <p className="field-description">Enter the name of the ingredient</p>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={ingredient.name}
                            onChange={handleInputChange}
                            placeholder="Enter ingredient name"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="category">Category</label>
                        <p className="field-description">Select or enter the ingredient category</p>
                        <select
                            id="category"
                            name="categoryId"
                            value={ingredient.categoryId}
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
                        <label htmlFor="unitPrice">Price (NOK)</label>
                        <p className="field-description">Enter the price per unit in Norwegian Krone</p>
                        <input
                            type="number"
                            id="unitPrice"
                            name="unitPrice"
                            value={ingredient.unitPrice}
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
                            checked={ingredient.isAvailable}
                            onChange={handleInputChange}
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="imageFile">Upload New Image</label>
                        <p className="field-description">Upload a new image for the ingredient (optional)</p>
                        <input
                            type="file"
                            id="imageFile"
                            name="imageFile"
                            onChange={handleFileChange}
                            accept="image/*"
                        />
                    </div>
                    <div className='modal-actions'>
                        <button type="submit" className="add-button">Save Changes</button>
                        <button type="button" className="cancel-button-ingredient-management" onClick={onClose}>Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default EditIngredientModal;