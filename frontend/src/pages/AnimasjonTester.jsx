import React, { useState } from "react";
import Navbar from "../shared/Navbar";
import DreamCup from "../components/animasjoner/DreamCup";
import DreamCup2dfull from "../components/animasjoner/DreamCup2dfull";
import DreamCup2dSeperateFill from "../components/animasjoner/DreamCup2dSeperateFill";
import "./AnimasjonTester.css"; // Import the CSS file

const Animasjon = () => {
    const [isAutoMode, setIsAutoMode] = useState(true); // Default to auto mode
    const [fillLevel, setFillLevel] = useState(0); // Default fill level
    const [view, setView] = useState("3D"); // Default view

    // Toggle auto mode
    const handleAutoMode = () => {
        setIsAutoMode(true);
        setFillLevel(0); // Reset fill level when switching to auto
    };

    // Enable manual mode and reset fill
    const handleManualMode = () => {
        setIsAutoMode(false);
        setFillLevel(0);
    };

    // Update fill level manually (only in manual mode)
    const handleFill = (amount) => {
        if (!isAutoMode) {
            setFillLevel(prev => Math.min(Math.max(prev + amount, 0), 100)); // Ensure range stays between 0-100
        }
    };

    // Render the selected view
    const renderView = () => {
        switch (view) {
            case "3D":
                return <DreamCup isAutoMode={isAutoMode} fillLevel={fillLevel} />;
            case "2D v2":
                return <DreamCup2dfull isAutoMode={isAutoMode} fillLevel={fillLevel} setFillLevel={setFillLevel} />;
            case "2D Separate Fill":
                return <DreamCup2dSeperateFill />;
            default:
                return <DreamCup isAutoMode={isAutoMode} fillLevel={fillLevel} />;
        }
    };

    return (
        <div>
            <Navbar />
            <div className="container-animasjon">
                {/* View Selection Buttons */}
                <div className="button-group">
                    <button onClick={() => setView("3D")} className="responsive-button">
                        3D
                    </button>
                    <button onClick={() => setView("2D v2")} className="responsive-button">
                        2D
                    </button>
                    <button onClick={() => setView("2D Separate Fill")} className="responsive-button">
                        2D Separate Fill
                    </button>
                </div>

                {/* Render the selected view */}
                <div className="svg-container">
                    {renderView()}
                </div>

                {/* Mode Selection Buttons */}
                <div className="button-group">
                    <button onClick={handleAutoMode} disabled={isAutoMode} className="responsive-button">
                        Start Auto Mode
                    </button>
                    <button onClick={handleManualMode} disabled={!isAutoMode} className="responsive-button">
                        Switch to Manual
                    </button>
                </div>

                {/* Manual Fill Buttons */}
                {!isAutoMode && (
                    <div className="manual-fill-buttons">
                        {/* Increase Buttons */}
                        <div className="button-group">
                            {[5, 10, 25, 50].map(amount => (
                                <button key={`increase-${amount}`} onClick={() => handleFill(amount)} className="responsive-button">
                                    +{amount}%
                                </button>
                            ))}
                        </div>

                        {/* Decrease Buttons */}
                        <div className="button-group">
                            {[5, 10, 25, 50].map(amount => (
                                <button key={`decrease-${amount}`} onClick={() => handleFill(-amount)} className="responsive-button">
                                    -{amount}%
                                </button>
                            ))}
                        </div>

                        {/* Reset Button */}
                        <button onClick={() => handleFill(-fillLevel)} className="responsive-button">
                            Reset to 0%
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Animasjon;