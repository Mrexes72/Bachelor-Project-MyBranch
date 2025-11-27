import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFacebook, faInstagram, faTwitter } from '@fortawesome/free-brands-svg-icons';


const SocialLinks = () => {
    const [hovered, setHovered] = useState(null);
    
    const links = [
        { name: 'Facebook', url: 'https://facebook.com', icon: faFacebook },
        { name: 'Instagram', url: 'https://instagram.com', icon: faInstagram },
        { name: 'Twitter', url: 'https://twitter.com', icon: faTwitter },
    ];

    return (
        <div style={{ textAlign: 'center', padding: '20px' }}>
            
            <div style={{ display: 'flex', justifyContent: 'center', gap: '20px' }}>
                {links.map((link, index) => (
                    <a
                        key={index}
                        href={link.url}
                        target="_blank"
                        rel="noopener noreferrer"
                        style={{
                            color: hovered === index ? '#0056b3' : '#007bff',
                            fontSize: '24px',
                            transition: 'color 0.3s ease',
                        }}
                        onMouseEnter={() => setHovered(index)}
                        onMouseLeave={() => setHovered(null)}
                    >
                        <FontAwesomeIcon icon={link.icon} />
                    </a>
                ))}
            </div>
        </div>
    );
};

export default SocialLinks;