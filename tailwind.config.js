/** @type {import('tailwindcss').Config} */
const plugin = require('tailwindcss/plugin')

module.exports = {
    content: ['./**/*.{razor,html}'],
    theme: {
        extend: {},
    },
    plugins: [
        require("@tailwindcss/typography"),
        require("tailwindcss-animate"),
        require("daisyui")
    ],
}

