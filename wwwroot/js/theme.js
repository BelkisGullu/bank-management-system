// Dark Mode / Light Mode Toggle

(function() {
    // Theme'i localStorage'dan oku veya varsayılan olarak 'light' kullan
    function getTheme() {
        return localStorage.getItem('theme') || 'light';
    }

    // Theme'i ayarla
    function setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
        updateThemeIcon(theme);
    }

    // Theme icon'unu güncelle
    function updateThemeIcon(theme) {
        const themeIcon = document.getElementById('themeIcon');
        if (themeIcon) {
            if (themeIcon.tagName === 'I') {
                // Font Awesome icon
                themeIcon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
            } else {
                // Fallback for text
                themeIcon.textContent = theme === 'dark' ? '☀️' : '🌙';
            }
        }
    }

    // Sayfa yüklendiğinde theme'i uygula
    document.addEventListener('DOMContentLoaded', function() {
        const currentTheme = getTheme();
        setTheme(currentTheme);

        // Toggle butonuna event listener ekle
        const themeToggle = document.getElementById('themeToggle');
        if (themeToggle) {
            themeToggle.addEventListener('click', function() {
                const currentTheme = getTheme();
                const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
                setTheme(newTheme);
            });
        }
    });
})();



