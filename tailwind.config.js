/** @type {import('tailwindcss').Config} */
const plugin = require('tailwindcss/plugin')

module.exports = {
    darkMode: 'class',
    content: [
        './SupasharpTodo.BlazorWASM/Pages/**/*.{razor,html}',
        './SupasharpTodo.BlazorWASM/Shared/**/*.{razor,html}',
        './SupasharpTodo.Shared/Components/**/*.{razor,html}',
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require("@tailwindcss/typography"),
        require("tailwindcss-animate"),
        require("daisyui")
    ],
}

