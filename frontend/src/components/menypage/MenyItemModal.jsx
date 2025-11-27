import React, { useEffect, useRef } from 'react';
import './MenyItemModal.css';

const MenuItemModal = ({ isOpen, onClose, menuItem }) => {
    const modalRef = useRef(null);

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (modalRef.current && !modalRef.current.contains(event.target)) {
                onClose();
            }
        };

        if (isOpen) {
            document.addEventListener('mousedown', handleClickOutside);
        }

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [isOpen, onClose]);

    if (!isOpen || !menuItem) return null;

    return (
        <div className="modal">
            <div className="modal-content" ref={modalRef}>
                <span className="close-button" onClick={onClose}>&times;</span>
                <h2>{menuItem.name}</h2>
                <img
                    src={menuItem.imagePath || '/images/default.png'}
                    alt={menuItem.name}
                    className="menu-item-image-modal"
                />
                <p className="menu-item-description">{menuItem.description}</p>
                <p className="menu-item-price"><strong>Pris:</strong> {menuItem.price} NOK</p>
                <button className="bottom-close-button" onClick={onClose}>Lukk</button>
            </div>
        </div>
    );
};

export default MenuItemModal;