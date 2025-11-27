import React from "react";
import './nyheter.css';
//import './styles.css'; // Import CSS


const Nyheter = () => {
  return (
    <aside className="sidebar">
      <div className="nyheter-container">
        <h2 className="nyheter-title">NYHETER / TILBUD</h2>

        <div className="nyheter-content">
          <div className="news-items">
            {[1, 2, 3, 4].map((item) => (
              <div key={item} className="item-container">
                <div className="item-box">
                  <span>ITEM {item}</span>
                </div>
                <div className="bilde-box">
                  <span>BILDE</span>
                </div>
              </div>
            ))}
          </div>

          <p className="news-text">
            LAG DIN EGEN FAVORITTDIRKK MED DRØMMEKOPPEN! VELG INGREDIENSER I SANNTID OG SE KOPPEN FYLLE SEG OPP.
            GLED DEG TIL EN UNIK INTERAKTIV OPPLEVELSE.
          </p>

          <button className="btn-pink">PRØV DRØMMEKOPPEN</button>
        </div>
      </div>
    </aside>
  );
};

export default Nyheter;
