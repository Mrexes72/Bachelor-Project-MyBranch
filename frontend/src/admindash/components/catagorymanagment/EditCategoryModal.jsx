import React from 'react';
import './CategoryModal.css';

const EditCategoryModal = ({ isOpen, onClose, onSubmit, category, handleInputChange }) => {
    if (!isOpen) return null;

    return (
        <div className="modal">
            <div className="modal-content-category-management">
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>Edit Category</h2>
                <form onSubmit={onSubmit} className="edit-category-form">
                    <div className="form-group">
                        <label htmlFor="name">Category Name</label>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={category.name}
                            onChange={handleInputChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="description">Description</label>
                        <textarea
                            id="description"
                            name="description"
                            value={category.description}
                            onChange={handleInputChange}
                        />
                    </div>
                    <div className='modal-actions'>
                        <button type="submit" className="save-button">Add Category</button>
                        <button type="button" className="cancel-button" onClick={onClose}>Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default EditCategoryModal;