import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import HomePage from './pages/HomePage';
import Meny from './pages/MenyPage';
import Login from './pages/Login';
import AdminDash from './admindash/Admindash';
import Drommekoppen from './pages/Drommekoppen';
/* import AnimasjonTester from './pages/AnimasjonTester'; */
import 'bootstrap/dist/css/bootstrap.min.css';
import ProtectedAdminRoute from './utils/ProtectedAdminRoute';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/meny" element={<Meny />} />
        <Route path="/drommekoppen" element={<Drommekoppen />} />
        <Route path="/login" element={<Login />} />
        <Route
          path="/admindash"
          element={
            <ProtectedAdminRoute>
              <AdminDash />
            </ProtectedAdminRoute>
          }
        />
        {/* <Route path="/animasjontester" element={<AnimasjonTester />} /> */}
      </Routes>
    </Router>
  );
}

export default App;