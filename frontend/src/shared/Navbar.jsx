import React, { useState } from "react";
import { NavLink } from "react-router-dom";
import { useUser } from "../components/UserContext";
import "./Navbar.css";

function Navbar() {
    const [menuOpen, setMenuOpen] = useState(false);
    const { user } = useUser();

    const handleToggle = () => {
        setMenuOpen(!menuOpen);
    };

    const handleLinkClick = () => {
        setMenuOpen(false);
    };

    // Check if the user is logged in and has role Admin
    const isAdmin = user?.role === 'Admin';

    return (
        <nav className="navbar">
            {/* LEFT: Brand + three main links (large screens) */}
            <div className="nav-left">
                <NavLink to="/" className="brand-title" onClick={handleLinkClick}>
                    Matcha og Mocha
                </NavLink>
                <ul className="main-links">
                    <li>
                        <NavLink
                            to="/"
                            className={({ isActive }) => (isActive ? "active" : "")}
                            onClick={handleLinkClick}
                        >
                            Hjem
                        </NavLink>
                    </li>
                    <li>
                        <NavLink
                            to="/meny"
                            className={({ isActive }) => (isActive ? "active" : "")}
                            onClick={handleLinkClick}
                        >
                            Meny
                        </NavLink>
                    </li>
                    <li>
                        <NavLink
                            to="/drommekoppen"
                            className={({ isActive }) => (isActive ? "active" : "")}
                            onClick={handleLinkClick}
                        >
                            Drømmekoppen
                        </NavLink>
                    </li>
                    {/* <li>
                        <NavLink
                            to="/animasjontester"
                            className={({ isActive }) => (isActive ? "active" : "")}
                            onClick={handleLinkClick}
                        >
                            AnimasjonsTester
                        </NavLink>
                    </li> */}
                    {isAdmin && (
                        <li>
                            <NavLink
                                to="/admindash"
                                onClick={handleLinkClick}
                            >
                                AdminDash
                            </NavLink>
                        </li>
                    )}
                </ul>
            </div>

            {/* RIGHT: Login + toggle button (hamburger) */}
            <div className="nav-right">
                <NavLink
                    to="/login"
                    className={({ isActive }) => "login-button " + (isActive ? "active" : "")}
                    onClick={handleLinkClick}
                >
                    {user ? "Min Side" : "Login"}
                </NavLink>

                <button className="toggle-button" onClick={handleToggle}>
                    ☰
                </button>
            </div>

            {/* DROPDOWN (mobile) */}
            <div className={`dropdown ${menuOpen ? "active" : ""}`}>
                <ul>
                    <li>
                        <NavLink
                            to="/"
                            className={({ isActive }) => (isActive ? "active" : "")}
                            onClick={handleLinkClick}
                        >
                            Hjem
                        </NavLink>
                    </li>
                    <li>
                        <NavLink
                            to="/meny"
                            className={({ isActive }) => (isActive ? "active" : "")}
                            onClick={handleLinkClick}
                        >
                            Meny
                        </NavLink>
                    </li>
                    <li>
                        <NavLink
                            to="/drommekoppen"
                            className={({ isActive }) => (isActive ? "active" : "")}
                            onClick={handleLinkClick}
                        >
                            Drømmekoppen
                        </NavLink>
                    </li>
                    <li>
                        <NavLink
                            to="/login"
                            className={({ isActive }) =>
                                "login-button " + (isActive ? "active" : "")
                            }
                            onClick={handleLinkClick}
                        >
                            {user ? "Min Side" : "Login"}
                        </NavLink>
                    </li>
                    {isAdmin && (
                        <li>
                            <NavLink to="/admindash" onClick={handleLinkClick}>AdminDash</NavLink>
                        </li>
                    )}
                    {/* <li>
                        <NavLink
                            to="/animasjontester"
                            className={({ isActive }) => (isActive ? "active" : "")}
                            onClick={handleLinkClick}
                        >
                            AnimasjonsTester
                        </NavLink>
                    </li> */}
                </ul>
            </div>
        </nav>
    );
}

export default Navbar;