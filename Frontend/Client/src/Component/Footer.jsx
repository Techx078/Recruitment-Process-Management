// Footer.jsx
import { FaFacebook, FaTwitter, FaInstagram, FaLinkedin } from "react-icons/fa";

export default function Footer() {
  return (
    <footer className="bg-gray-900 text-gray-300 w-full mt-10 ">
      {/* Main content of footer */}
      <div className="w-full mr-3">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 grid grid-cols-1 md:grid-cols-3 gap-8">
        {/* About Section */}
        <div>
          <h3 className="text-xl font-semibold text-white mb-4">About</h3>
          <ul className="space-y-2">
            <li>
              <a href="#" className="hover:text-indigo-500 transition-colors">
                About us
              </a>
            </li>
            <li>
              <a href="#" className="hover:text-indigo-500 transition-colors">
                Our values
              </a>
            </li>
            <li>
              <a href="#" className="hover:text-indigo-500 transition-colors">
                Open position
              </a>
            </li>
            <li>
              <a href="#" className="hover:text-indigo-500 transition-colors">
                Contact
              </a>
            </li>
          </ul>
        </div>

        {/* Social Media */}
        <div>
          <h3 className="text-xl font-semibold text-white mb-4">Follow Us</h3>
          <div className="flex space-x-4 ml-29">
            <a href="#" className="hover:text-indigo-500 transition-colors">
              <FaFacebook size={24} />
            </a>
            <a href="#" className="hover:text-indigo-500 transition-colors">
              <FaTwitter size={24} />
            </a>
            <a href="#" className="hover:text-indigo-500 transition-colors">
              <FaInstagram size={24} />
            </a>
            <a href="#" className="hover:text-indigo-500 transition-colors">
              <FaLinkedin size={24} />
            </a>
          </div>
        </div>
      </div>

      {/* Bottom copyright */}
      <div className="bg-gray-800 text-gray-500 text-center py-4">
        &copy; {new Date().getFullYear()} MyLogo. All rights reserved.
      </div>
      </div>
    </footer>
  );
}
