import React, { useEffect } from 'react';
import './PriceBox.css';

const PriceBox = ({ selectedIngredients, onTotalPriceChange }) => {
    const totalPrice = selectedIngredients.reduce((total, ingredient) => total + ingredient.unitPrice, 0);

    useEffect(() => {
        if (onTotalPriceChange) {
            onTotalPriceChange(totalPrice); // Notify parent of the total price
        }
    }, [totalPrice, onTotalPriceChange]);

    return (
        <div className="box pris">
            <h2>Pris</h2>
            {selectedIngredients.length > 0 ? (
                <>
                    <table className="price-table">
                        <tbody>
                            {selectedIngredients.map((ingredient, index) => (
                                <tr key={index}>
                                    <td>{ingredient.name}</td>
                                    <td>{ingredient.unitPrice} kr</td>
                                </tr>
                            ))}
                            <tr>
                                <td colSpan="2" className="dashed-line"></td>
                            </tr>
                            <tr>
                                <td><strong>Total:</strong></td>
                                <td><strong>{totalPrice} kr</strong></td>
                            </tr>
                            <tr>
                                <td colSpan="2" className="dashed-line"></td>
                            </tr>
                        </tbody>
                    </table>
                </>
            ) : (
                <p className="filler-text">Ingen valgt</p>
            )}
        </div>
    );
};

export default PriceBox;