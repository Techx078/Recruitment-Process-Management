// Navbar.jsx
import { useState } from "react";
import { HiMenu, HiX, HiChevronDown } from "react-icons/hi";
import { Link } from "react-router-dom";
import roimalogo from '../../Public/roima.png';

export default function Navbar() {
  const [menuOpen, setMenuOpen] = useState(false);
  const [servicesOpen, setServicesOpen] = useState(false); // For dropdown

  const toggleMenu = () => setMenuOpen(!menuOpen);
  const toggleServices = () => setServicesOpen(!servicesOpen);

  return (
    <nav className="bg-white shadow-md fixed w-full z-50 mb-5">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-20 items-center">
          {/* Logo */}
          <div className="flex-shrink-0 ">
             <Link to="/" className="flex items-center space-x-3">
          <img
            src={roimalogo}
            alt="Roima Logo"
            className="h-10 w-auto object-contain"
          />
          </Link>
          </div>

          {/* Desktop Menu */}
          <div className="hidden md:flex space-x-6 items-center">
            <Link to="/" className="text-gray-700 hover:text-indigo-600">
              Home
            </Link>
            <Link to="/Login" className="text-gray-700 hover:text-indigo-600">
              Login
            </Link>
            <Link to="/Candidate-register" className="text-gray-700 hover:text-indigo-600">
              Candidate-Register
            </Link>
            <Link to="/Other-Register" className="text-gray-700 hover:text-indigo-600">
              Other-Register
            </Link>
          </div>

          {/* Mobile Menu Button */}
          <div className="md:hidden">
            <button
              onClick={toggleMenu}
              className="text-gray-700 hover:text-indigo-600 focus:outline-none"
            >
              {menuOpen ? <HiX size={28} /> : <HiMenu size={28} />}
            </button>
          </div>
        </div>
      </div>

      {/* Mobile Menu */}
      <div
        className={`md:hidden bg-white shadow-md overflow-hidden transition-all duration-300 ${
          menuOpen ? "max-h-screen" : "max-h-0"
        }`}
      >
        <Link
          to="/"
          className="block px-4 py-3 text-gray-700 hover:bg-indigo-50 hover:text-indigo-600"
        >
          Home
        </Link>
        <Link
          to="/Login"
          className="block px-4 py-3 text-gray-700 hover:bg-indigo-50 hover:text-indigo-600"
        >
          Login
        </Link>
         <Link
          to="/Candidate-Register"
          className="block px-4 py-3 text-gray-700 hover:bg-indigo-50 hover:text-indigo-600"
        >
          Candidate-Register
        </Link>
        <Link to="/Other-register" className="text-gray-700 hover:text-indigo-600">
              Other-Register
        </Link>
      </div>
      
    </nav>
  );
}
