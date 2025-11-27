import React, { useState, useEffect, useRef } from "react";
import './meny.css';
import { fetchMenuItems } from '../../shared/apiConfig';
import MenuItemModal from "./MenyItemModal";

const Meny = () => {
    const [activeTab, setActiveTab] = useState('kaffe');
    const [dropdownTab, setDropdownTab] = useState('');
    const [varmDrikke, setVarmDrikke] = useState([]);
    const [kaldDrikke, setKaldDrikke] = useState([]);
    const [smoothie, setSmoothie] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [selectedMenuItem, setSelectedMenuItem] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const dropdownRef = useRef(null);

    //Fetch menu items for kaffevarm as default
    useEffect(() => {
        handleFetchMenuItems('KaffeVarm');
    }, []);

    const handleFetchMenuItems = async (category) => {
        setLoading(true);
        console.log(`Fetching menu items for category: ${category}`);
        setError(null);

        // Clear other category states
        setVarmDrikke([]);
        setKaldDrikke([]);
        setSmoothie([]);

        try {
            const data = await fetchMenuItems(category); // Fetch all menu items from the backend
            console.log(`Fetched Data:`, data);

            // Filter and sort menu items by category in the frontend
            const filteredData = data
                .filter(item => item.categoryName === category && item.isAvailable) // Filter by category
                .sort((a, b) => a.name.localeCompare(b.name)); // Sort alphabetically by name

            if (category === 'KaffeVarm') {
                setVarmDrikke(filteredData);
            } else if (category === 'KaffeKald') {
                setKaldDrikke(filteredData);
            } else if (category === 'Smoothie') {
                setSmoothie(filteredData);
            } else if (category === 'MatchaVarm') {
                setVarmDrikke(filteredData);
            } else if (category === 'MatchaKald') {
                setKaldDrikke(filteredData);
            }
        } catch (error) {
            console.error(`Error fetching menu items for category ${category}:`, error);
            setError(error.message);
        } finally {
            setLoading(false);
        }
    };

    const dropdownContent = {
        kaffe: [
            { name: "VARME DRIKKER", action: () => handleFetchMenuItems('KaffeVarm') },
            { name: "KALDE DRIKKER", action: () => handleFetchMenuItems('KaffeKald') },
        ],
        matcha: [
            { name: "VARME DRIKKER", action: () => handleFetchMenuItems('MatchaVarm') },
            { name: "KALDE DRIKKER", action: () => handleFetchMenuItems('MatchaKald') },
        ],
        smoothie: [
            { name: "Smoothie", action: () => handleFetchMenuItems('Smoothie') },
        ],
    };

    const handleMenuItemClick = (menuItem) => {
        setSelectedMenuItem(menuItem);
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setSelectedMenuItem(null);
        setIsModalOpen(false);
    };

    const handleTabClick = (tab) => {
        if (dropdownTab === tab) {
            setDropdownTab(null); // Close dropdown if it's already open
        } else {
            setDropdownTab(tab); // Open the clicked dropdown
        }
        setActiveTab(tab); // Set the active tab
    };

    const handleClickOutside = (event) => {
        if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
            setDropdownTab(null); // Close dropdown if clicked outside
        }
    };

    useEffect(() => {
        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    return (
        <main className="menu">
            <div className="menu-box">
                <div className="menu-description">
                    <h2>MENY</h2>
                    <p>Trykk på en kategori for å se hva vi har i den kategorien.</p>
                </div>
                <div className="menu-header">
                    {Object.keys(dropdownContent).map((tab) => (
                        <div key={tab} className="menu-tab">
                            <div
                                className={`menu-tab-header ${activeTab === tab ? "active" : ""}`}
                                onClick={() => handleTabClick(tab)}
                            >
                                {tab.toUpperCase()}
                            </div>
                            {dropdownTab === tab && (
                                <div className="dropdown-menu" ref={dropdownRef}>
                                    {dropdownContent[tab].map((item, index) => (
                                        <div
                                            key={index}
                                            className="dropdown-item"
                                            onClick={() => {
                                                item.action && item.action();
                                                setDropdownTab(null); // Close dropdown after selecting an item
                                            }}
                                        >
                                            {item.name}
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    ))}
                </div>

                <div className="menu-pictures">
                    {loading ? (
                        <p>Loading...</p>
                    ) : error ? (
                        <p>Error: {error}</p>
                    ) : (
                        <>
                            {(varmDrikke || []).length > 0 && (
                                <div>
                                    <h3>Varme Drikker</h3>
                                    <div className="menu-items">
                                        {(varmDrikke || []).map((menuItem, index) => (
                                            <div
                                                key={index}
                                                className="menu-item"
                                                onClick={() => handleMenuItemClick(menuItem)}
                                            >
                                                <img
                                                    src={menuItem.imagePath || '/images/default.png'}
                                                    alt={menuItem.name}
                                                    className="menu-item-image"
                                                />
                                                <p>{menuItem.name}</p>
                                                <p className="menu-item-price"><strong>Pris:</strong> {menuItem.price} NOK</p>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            )}
                            {(kaldDrikke || []).length > 0 && (
                                <div>
                                    <h3>Kalde Drikker</h3>
                                    <div className="menu-items">
                                        {(kaldDrikke || []).map((menuItem, index) => (
                                            <div
                                                key={index}
                                                className="menu-item"
                                                onClick={() => handleMenuItemClick(menuItem)}
                                            >
                                                <img
                                                    src={menuItem.imagePath || '/images/default.png'}
                                                    alt={menuItem.name}
                                                    className="menu-item-image"
                                                />
                                                <p>{menuItem.name}</p>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            )}
                            {(smoothie || []).length > 0 && (
                                <div>
                                    <h3>Smoothie</h3>
                                    <div className="menu-items">
                                        {(smoothie || []).map((menuItem, index) => (
                                            <div
                                                key={index}
                                                className="menu-item"
                                                onClick={() => handleMenuItemClick(menuItem)}
                                            >
                                                <img
                                                    src={menuItem.imagePath || '/images/default.png'}
                                                    alt={menuItem.name}
                                                    className="menu-item-image"
                                                />
                                                <p>{menuItem.name}</p>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            )}
                        </>
                    )}
                </div>
            </div>
            {/* <div className="button-group">
                <button className="btn-green">BESTILL MED FOODORA</button>
                <button className="btn-blue">BESTILL MED WOLT</button>
            </div> */}
            {/* MenuItemModal */}
            <MenuItemModal
                isOpen={isModalOpen}
                onClose={closeModal}
                menuItem={selectedMenuItem}
            />
        </main>
    );
};

export default Meny;