// ========================================
// BANK MANAGEMENT SYSTEM - ANIMATIONS JS
// ========================================

(function() {
    'use strict';

    // 1. PAGE LOAD ANIMATION - Sayfa Yükleme Animasyonu
    document.addEventListener('DOMContentLoaded', function() {
        document.body.classList.add('fade-in');
    });

    // 2. RIPPLE EFFECT - Buton Ripple Efekti
    function addRippleEffect() {
        const buttons = document.querySelectorAll('.btn:not(.no-ripple)');
        
        buttons.forEach(button => {
            button.addEventListener('click', function(e) {
                const ripple = document.createElement('span');
                const rect = this.getBoundingClientRect();
                const size = Math.max(rect.width, rect.height);
                const x = e.clientX - rect.left - size / 2;
                const y = e.clientY - rect.top - size / 2;
                
                ripple.style.width = ripple.style.height = size + 'px';
                ripple.style.left = x + 'px';
                ripple.style.top = y + 'px';
                ripple.classList.add('ripple');
                
                this.appendChild(ripple);
                
                setTimeout(() => {
                    ripple.remove();
                }, 600);
            });
        });
    }

    // 3. FORM SUBMIT LOADING - Form Gönderme Yükleme
    function addFormLoading() {
        const forms = document.querySelectorAll('form');
        
        forms.forEach(form => {
            form.addEventListener('submit', function(e) {
                const submitButton = form.querySelector('button[type="submit"]');
                
                if (submitButton && !submitButton.classList.contains('loading')) {
                    submitButton.classList.add('loading');
                    submitButton.disabled = true;
                    
                    // Eğer form validation hatası varsa loading'i kaldır
                    setTimeout(() => {
                        if (!form.checkValidity()) {
                            submitButton.classList.remove('loading');
                            submitButton.disabled = false;
                        }
                    }, 100);
                }
            });
        });
    }

    // 4. TOAST NOTIFICATION SYSTEM - Bildirim Sistemi
    function showToast(message, type = 'info', duration = 3000) {
        // Toast container oluştur
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        // Toast elementi oluştur
        const toast = document.createElement('div');
        toast.className = `alert alert-${type} alert-dismissible fade show toast`;
        toast.setAttribute('role', 'alert');
        
        const icon = type === 'success' ? '✅' : type === 'danger' ? '❌' : type === 'warning' ? '⚠️' : 'ℹ️';
        toast.innerHTML = `
            <strong>${icon}</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;

        container.appendChild(toast);

        // Otomatik kapatma
        setTimeout(() => {
            toast.classList.add('hiding');
            setTimeout(() => {
                toast.remove();
            }, 300);
        }, duration);

        // Manuel kapatma
        toast.querySelector('.btn-close')?.addEventListener('click', function() {
            toast.classList.add('hiding');
            setTimeout(() => {
                toast.remove();
            }, 300);
        });
    }

    // Global olarak erişilebilir yap
    window.showToast = showToast;

    // 5. NUMBER COUNTER ANIMATION - Sayı Sayma Animasyonu
    function animateCounter(element, target, duration = 2000) {
        const start = 0;
        const increment = target / (duration / 16); // 60 FPS
        let current = start;

        const timer = setInterval(() => {
            current += increment;
            if (current >= target) {
                element.textContent = Math.floor(target);
                clearInterval(timer);
            } else {
                element.textContent = Math.floor(current);
            }
        }, 16);
    }

    window.animateCounter = animateCounter;

    // 6. SMOOTH SCROLL TO TOP - Yukarı Kaydırma
    function createScrollToTop() {
        const scrollButton = document.createElement('button');
        scrollButton.innerHTML = '↑';
        scrollButton.className = 'btn btn-primary rounded-circle position-fixed';
        scrollButton.style.cssText = 'bottom: 30px; right: 30px; width: 50px; height: 50px; z-index: 1000; display: none;';
        scrollButton.setAttribute('aria-label', 'Yukarı git');
        document.body.appendChild(scrollButton);

        window.addEventListener('scroll', function() {
            if (window.pageYOffset > 300) {
                scrollButton.style.display = 'block';
                scrollButton.classList.add('fade-in');
            } else {
                scrollButton.style.display = 'none';
            }
        });

        scrollButton.addEventListener('click', function() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    // 7. TABLE ROW ANIMATION - Tablo Satır Animasyonu
    function animateTableRows() {
        const rows = document.querySelectorAll('.table tbody tr');
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.animationPlayState = 'running';
                }
            });
        }, { threshold: 0.1 });

        rows.forEach(row => observer.observe(row));
    }

    // 8. INPUT VALIDATION ANIMATION - Input Doğrulama Animasyonu
    function addInputValidationAnimation() {
        const inputs = document.querySelectorAll('.form-control, .form-select');
        
        inputs.forEach(input => {
            input.addEventListener('invalid', function() {
                this.classList.add('shake');
                setTimeout(() => {
                    this.classList.remove('shake');
                }, 500);
            });

            input.addEventListener('input', function() {
                if (this.classList.contains('shake')) {
                    this.classList.remove('shake');
                }
            });
        });
    }

    // 9. CARD HOVER ENHANCEMENT - Kart Hover Geliştirmesi
    function enhanceCardHover() {
        const cards = document.querySelectorAll('.card');
        
        cards.forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.style.transition = 'all 0.3s ease';
            });
        });
    }

    // 10. SUCCESS MESSAGE AUTO-HIDE - Başarı Mesajı Otomatik Gizleme
    function autoHideSuccessMessages() {
        const successMessages = document.querySelectorAll('.alert-success, .message.success');
        
        successMessages.forEach(message => {
            setTimeout(() => {
                message.style.transition = 'opacity 0.5s ease-out';
                message.style.opacity = '0';
                setTimeout(() => {
                    message.remove();
                }, 500);
            }, 5000);
        });
    }

    // Initialize all animations
    document.addEventListener('DOMContentLoaded', function() {
        addRippleEffect();
        addFormLoading();
        createScrollToTop();
        animateTableRows();
        addInputValidationAnimation();
        enhanceCardHover();
        autoHideSuccessMessages();

        // Login form için özel loading overlay
        const loginForm = document.getElementById('loginForm');
        if (loginForm) {
            loginForm.addEventListener('submit', function(e) {
                // Form validation geçerse loading göster
                if (this.checkValidity()) {
                    showLoadingOverlay('Giriş yapılıyor...');
                }
            });
        }

        // Diğer formlar için de loading ekle (Customer Edit/Create hariç - overlay sorunları nedeniyle)
        const createForms = document.querySelectorAll('form[action*="Create"], form[action*="Edit"]');
        createForms.forEach(form => {
            // Customer formlarını atla
            if (form.action.includes('/Customer/Edit') || form.action.includes('/Customer/Create')) {
                return;
            }
            
            form.addEventListener('submit', function(e) {
                if (this.checkValidity()) {
                    const action = form.action.includes('Create') ? 'Kaydediliyor...' : 'Güncelleniyor...';
                    showLoadingOverlay(action);
                }
            });
        });
    });

    // Loading Overlay Function
    function showLoadingOverlay(message = 'Yükleniyor...') {
        const overlay = document.createElement('div');
        overlay.className = 'loading-overlay';
        overlay.innerHTML = `
            <div class="text-center text-white">
                <div class="spinner mb-3"></div>
                <p>${message}</p>
            </div>
        `;
        document.body.appendChild(overlay);
    }

    window.showLoadingOverlay = showLoadingOverlay;
    window.hideLoadingOverlay = function() {
        const overlay = document.querySelector('.loading-overlay');
        if (overlay) {
            overlay.style.animation = 'fadeOut 0.3s ease-out';
            setTimeout(() => overlay.remove(), 300);
        }
    };

})();


