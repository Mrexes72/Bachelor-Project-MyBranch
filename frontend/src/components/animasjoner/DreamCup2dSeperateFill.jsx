import React, { useEffect, useState, useRef, useCallback } from "react";
import gsap from "gsap";
import "./DreamCup2dSeperateFill.css";

const FILL_PATHS = {
    "Fill-1": "M97 359H188.295H299L297.778 377H98L97 359Z",
    "Fill-2": "M96 345H188.199H300L298.766 363H97.0099L96 345Z",
    "Fill-3": "M95 331H188.103H301L299.754 349H96.0198L95 331Z",
    "Fill-4": "M93 317H187.459H302L300.736 335H94.0347L93 317Z",
    "Fill-5": "M92 303H187.815H304L302.717 321H93.0495L92 303Z",
    "Fill-6": "M91 289H187.719H305L303.705 307H92.0594L91 289Z",
    "Fill-7": "M90 275H187.623H306L304.693 293H91.0693L90 275Z",
    "Fill-8": "M89 261H187.527H307L305.681 279H90.0792L89 261Z",
    "Fill-9": "M87 247H186.883H308L306.663 265H88.0941L87 247Z",
    "Fill-10": "M86 233H186.787H309L307.651 251H87.104L86 233Z",
    "Fill-11": "M85 219H187.142H311L309.633 237H86.1188L85 219Z",
    "Fill-12": "M83 205H186.498H312L310.615 223H84.1337L83 205Z",
    "Fill-13": "M83 191H186.95H313L311.609 209H84.1386L83 191Z",
    "Fill-14": "M82 177H186.854H314L312.597 195H83.1485L82 177Z",
    "Fill-15": "M80 163H186.21H315L313.578 181H81.1634L80 163Z",
    "Fill-16": "M79 149H186.114H316L314.566 167H80.1733L79 149Z",
    "Fill-17": "M78 135H186.47H318L316.548 153H79.1881L78 135Z",
    "Fill-18": "M77 121H186.374H319L317.536 139H78.198L77 121Z",
    "Fill-19": "M76 107H186.278H320L318.524 125H77.2079L76 107Z",
    "Fill-20": "M74 97H185.634H321L319.506 111H75.2228L74 97Z",
};

const DreamCup2dSeperateFill = ({ onAddIngredient, onRemoveIngredient, onResetLayers, setSvgRef, cupTheme, layers, setLayers }) => {
    const [currentStreamColor, setCurrentStreamColor] = useState("transparent");
    const newlyAddedRefs = useRef([]);
    const svgRef = useRef(null);

    const addIngredient = useCallback((ingredient) => {
        if (!ingredient || !ingredient.fillLevel || !ingredient.color) {
            console.warn("Invalid ingredient passed to addIngredient:", ingredient);
            return;
        }

        const amount = Math.floor(ingredient.fillLevel / 5);
        const newLayer = { color: ingredient.color, ingredient: ingredient.name };
        setCurrentStreamColor(ingredient.color);

        const newIndices = [];
        setLayers((prev) => {
            const copy = [...prev];
            let filled = 0;
            for (let i = 19; i >= 0 && filled < amount; i--) {
                if (!copy[i]) {
                    copy[i] = newLayer;
                    newIndices.push(i);
                    filled++;
                }
            }
            newlyAddedRefs.current = newIndices;
            return copy;
        });

        // Stream animation
        gsap.fromTo(
            "#liquid-stream-2dLayers",
            { opacity: 0 },
            {
                opacity: 1,
                duration: 1,
                ease: "power2.out",
                onComplete: () => {
                    gsap.to("#liquid-stream-2dLayers", {
                        opacity: 0,
                        duration: 2,
                        ease: "power2.out",
                    });
                },
            }
        );

        // Straw wiggle animation
        gsap.to("#Straw-layer", {
            rotation: 3,
            transformOrigin: "bottom",
            duration: 1.5,
            yoyo: true,
            repeat: 3,
            ease: "power1.inOut",
        });

        gsap.to("#StrawAcsent-layer", {
            rotation: 3,
            transformOrigin: "bottom",
            duration: 1.5,
            yoyo: true,
            repeat: 3,
            ease: "power1.inOut",
        });
    }, [setLayers]);

    const removeIngredient = useCallback((ingredient) => {
        if (!ingredient || !ingredient.fillLevel) {
            console.warn("Invalid ingredient passed to removeIngredient:", ingredient);
            return;
        }

        const amount = Math.floor(ingredient.fillLevel / 5);

        setLayers((prev) => {
            const copy = [...prev];
            let removed = 0;

            // Remove the ingredient and clear the corresponding layers
            for (let i = 19; i >= 0 && removed < amount; i--) {
                if (copy[i]?.ingredient === ingredient.name) {
                    copy[i] = null; // Clear the layer
                    removed++;
                }
            }

            // Compact the array by removing all null values and shifting layers down
            const compacted = copy.filter((layer) => layer !== null);

            // Fill the remaining spots with null to maintain the array length
            while (compacted.length < 20) {
                compacted.unshift(null); // Add nulls to the top to maintain bottom-to-top stacking
            }

            // Animate falling for layers that have shifted
            compacted.forEach((layer, newIndex) => {
                if (layer) {
                    const oldIndex = copy.findIndex((l) => l === layer);
                    if (oldIndex !== newIndex) {
                        const id = `#Fill-${20 - oldIndex}`;
                        const deltaY = (newIndex - oldIndex) * 14; // Adjust the Y-pixel distance per layer
                        gsap.fromTo(
                            id,
                            { y: -deltaY }, // Start from the offset position
                            {
                                y: 0, // Reset to the correct position
                                duration: 0.4,
                                ease: "power2.out",
                            }
                        );
                    }
                }
            });

            return compacted;
        });
    }, [setLayers]);

    const resetLayers = useCallback(() => {
        // Reset all layers to null and set opacity to 0
        setLayers(Array(20).fill(null));

        // Reset the opacity of all layers using GSAP
        for (let i = 1; i <= 20; i++) {
            const id = `#Fill-${i}`;
            gsap.to(id, {
                opacity: 0,
                duration: 0.5,
                ease: "power2.out",
            });
        }
    }, [setLayers]);

    // Expose addIngredient and removeIngredient to the parent
    useEffect(() => {
        if (onAddIngredient) {
            onAddIngredient(() => addIngredient);
        }
        if (onRemoveIngredient) {
            onRemoveIngredient(() => removeIngredient);
        }
        if (onResetLayers) {
            onResetLayers(() => resetLayers);
        }
    }, [onAddIngredient, onRemoveIngredient, onResetLayers, addIngredient, removeIngredient, resetLayers]);

    // Animate newly added layers
    useEffect(() => {
        if (newlyAddedRefs.current.length > 0) {
            const sorted = [...newlyAddedRefs.current].sort((a, b) => b - a);
            sorted.forEach((i, idx) => {
                const id = `#Fill-${20 - i}`;
                gsap.fromTo(id,
                    { opacity: 0, scaleY: 0.1, transformOrigin: "bottom" },
                    {
                        opacity: 1,
                        scaleY: 1,
                        duration: 1.2,
                        ease: "power2.out",
                        delay: idx * 0.3,
                    }
                );
            });
            newlyAddedRefs.current = [];
        }
    }, [layers]);

    // Expose the SVG reference to the parent
    useEffect(() => {
        if (setSvgRef) {
            setSvgRef(svgRef);
        }
    }, [setSvgRef]);

    return (
        <div className="svg-container-2d-SeperateFill">
            <svg
                ref={svgRef} // Attach the reference
                width="100%"
                height="100%"
                viewBox="0 0 400 400"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
            >
                <defs>
                    {/* Define the blur filter */}
                    <filter id="blur-filter" x="0" y="0" width="100%" height="100%">
                        <feGaussianBlur in="SourceGraphic" stdDeviation="4" />
                    </filter>
                </defs>

                <rect width="300" height="300" fill="none" />

                <line
                    id="Straw-layer"
                    x1="123.966" y1="16.3095" x2="157.966" y2="359.309"
                    stroke="#E7D8BA"
                    strokeWidth="14"
                />
                {/* Straw Accent */}
                <line
                    id="StrawAcsent-layer"
                    x1="123.966"
                    y1="16.3095"
                    x2="157.966"
                    y2="359.309"
                    stroke={cupTheme.straw} // Use the straw accent color from the theme
                    strokeWidth="14"
                    strokeDasharray="10 10"
                />
                <path
                    id="liquid-stream-2dLayers"
                    d="M200, -20 L200,370"
                    stroke={currentStreamColor}
                    strokeWidth="14"
                    strokeLinecap="round"
                    opacity="0"
                />

                {/* Apply the blur filter to the layers */}
                <g filter="url(#blur-filter)">
                    {layers.map((layer, index) => {
                        const id = `Fill-${20 - index}`;
                        return (
                            <path
                                key={id}
                                id={id}
                                d={FILL_PATHS[id] || ""}
                                fill={layer ? layer.color : "transparent"} // Use "transparent" for null layers
                                opacity={layer ? 1 : 0} // Set opacity to 0 for null layers
                                className="cup-fill-layer"
                            />
                        );
                    })}
                </g>

                {/* Cup Outline */}
                <path
                    id="Cup-Outline"
                    d="M321.186 90.9364L298.637 366.936C298.15 372.905 293.164 377.5 287.176 377.5H108.78C102.806 377.5 97.8266 372.926 97.3208 366.974L73.8698 90.9736C73.2996 84.2624 78.5932 78.5 85.3286 78.5H188H309.725C316.445 78.5 321.734 84.2382 321.186 90.9364Z"
                    stroke={cupTheme.cup} // Use the cup color from the theme
                    strokeWidth="10"
                />
                {/* Lid */}
                <g id="Lid-bot">
                    <path
                        d="M64.3455 78.3866C65.0359 76.3614 66.9384 75 69.078 75H326.922C329.062 75 330.964 76.3614 331.655 78.3866L335.745 90.3866C336.851 93.6304 334.44 97 331.013 97H64.9871C61.56 97 59.1487 93.6304 60.2546 90.3866L64.3455 78.3866Z"
                        fill={cupTheme.lid} // Use the lid color from the theme
                    />
                </g>
                <g id="Lid-top">
                    <path
                        d="M83.5375 57.8811C84.059 55.6099 86.0803 54 88.4107 54H307.589C309.92 54 311.941 55.6099 312.463 57.8812L316.595 75.8812C317.314 79.0128 314.935 82 311.722 82H84.278C81.0649 82 78.6858 79.0128 79.4048 75.8812L83.5375 57.8811Z"
                        fill={cupTheme.lid} // Use the lid color from the theme
                    />
                </g>
            </svg>
        </div>
    );
};

export default DreamCup2dSeperateFill;