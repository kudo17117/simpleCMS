// ✅ site.js is loaded and running!
(function () {
    "use strict";

    // Scroll toggle
    function toggleScrolled() {
        const body = document.querySelector('body');
        const header = document.querySelector('#header');
        if (!header.classList.contains('scroll-up-sticky') &&
            !header.classList.contains('sticky-top') &&
            !header.classList.contains('fixed-top')) return;
        window.scrollY > 100 ? body.classList.add('scrolled') : body.classList.remove('scrolled');
    }
    document.addEventListener('scroll', toggleScrolled);
    window.addEventListener('load', toggleScrolled);

    // Mobile nav toggle
    const mobileNavToggleBtn = document.querySelector('.mobile-nav-toggle');
    function mobileNavToogle() {
        document.body.classList.toggle('mobile-nav-active');
        mobileNavToggleBtn.classList.toggle('bi-list');
        mobileNavToggleBtn.classList.toggle('bi-x');
    }
    mobileNavToggleBtn?.addEventListener('click', mobileNavToogle);

    // Hide nav on hash click
    document.querySelectorAll('#navmenu a').forEach(link => {
        link.addEventListener('click', () => {
            if (document.querySelector('.mobile-nav-active')) {
                mobileNavToogle();
            }
        });
    });

    // Dropdowns
    document.querySelectorAll('.navmenu .toggle-dropdown').forEach(el => {
        el.addEventListener('click', function (e) {
            e.preventDefault();
            this.parentNode.classList.toggle('active');
            this.parentNode.nextElementSibling.classList.toggle('dropdown-active');
            e.stopImmediatePropagation();
        });
    });

    // Preloader
    const preloader = document.querySelector('#preloader');
    if (preloader) {
        window.addEventListener('load', () => preloader.remove());
    }

    // Scroll top button
    const scrollTop = document.querySelector('.scroll-top');
    function toggleScrollTop() {
        if (scrollTop) {
            window.scrollY > 100 ? scrollTop.classList.add('active') : scrollTop.classList.remove('active');
        }
    }
    scrollTop?.addEventListener('click', e => {
        e.preventDefault();
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });
    window.addEventListener('load', toggleScrollTop);
    document.addEventListener('scroll', toggleScrollTop);

    // AOS animation
    function aosInit() {
        AOS.init({ duration: 600, easing: 'ease-in-out', once: true, mirror: false });
    }
    window.addEventListener('load', aosInit);

    // Lightbox
    GLightbox({ selector: '.glightbox' });

    // Counter
    new PureCounter();

    // FAQ toggle
    document.querySelectorAll('.faq-item h3, .faq-item .faq-toggle').forEach(el => {
        el.addEventListener('click', () => {
            el.parentNode.classList.toggle('faq-active');
        });
    });

    // Swiper init
    function initSwiper() {
        document.querySelectorAll(".init-swiper").forEach(swiper => {
            let config = JSON.parse(swiper.querySelector(".swiper-config").innerHTML.trim());
            new Swiper(swiper, config);
        });
    }
    window.addEventListener("load", initSwiper);

    // Hash scroll fix
    window.addEventListener('load', () => {
        if (window.location.hash && document.querySelector(window.location.hash)) {
            setTimeout(() => {
                const section = document.querySelector(window.location.hash);
                const offset = parseInt(getComputedStyle(section).scrollMarginTop);
                window.scrollTo({ top: section.offsetTop - offset, behavior: 'smooth' });
            }, 100);
        }
    });

    // Scrollspy
    const navmenulinks = document.querySelectorAll('.navmenu a');
    function navmenuScrollspy() {
        navmenulinks.forEach(link => {
            if (!link.hash) return;
            const section = document.querySelector(link.hash);
            if (!section) return;
            const pos = window.scrollY + 200;
            if (pos >= section.offsetTop && pos <= (section.offsetTop + section.offsetHeight)) {
                document.querySelectorAll('.navmenu a.active').forEach(l => l.classList.remove('active'));
                link.classList.add('active');
            } else {
                link.classList.remove('active');
            }
        });
    }
    window.addEventListener('load', navmenuScrollspy);
    document.addEventListener('scroll', navmenuScrollspy);
})();

// 🧩 Wait helper
function waitFor(conditionFn, timeout = 3000, interval = 100) {
    return new Promise((resolve, reject) => {
        const start = Date.now();
        (function check() {
            if (conditionFn()) return resolve();
            if (Date.now() - start >= timeout) return reject("Timeout waiting for condition");
            setTimeout(check, interval);
        })();
    });
}

// 🔍 Autocomplete + Autofill via AJAX + Cascading
document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("searchQuery");
    const resultsList = document.getElementById("searchResults");

    const regionSelect = document.getElementById("region");
    const provinceSelect = document.getElementById("province");
    const municipalitySelect = document.getElementById("municipality");
    const clinicSelect = document.getElementById("clinic");
    const doctorSelect = document.getElementById("doctor");

    if (!searchInput || !resultsList) return;

    searchInput.addEventListener("input", function () {
        const query = this.value.trim();
        if (query.length < 2) {
            resultsList.style.display = "none";
            resultsList.innerHTML = "";
            return;
        }

        fetch(`/Appointment/Autocomplete?term=${encodeURIComponent(query)}`)
            .then(response => response.ok ? response.json() : [])
            .then(data => {
                resultsList.innerHTML = "";

                if (!data || data.length === 0) {
                    resultsList.innerHTML = '<li class="list-group-item text-danger">No clinic found</li>';
                    resultsList.style.display = "block";
                    const submitBtn = document.getElementById("submitAppointment");
                    if (submitBtn) submitBtn.disabled = true;
                    return;
                }

                data.forEach(item => {
                    const li = document.createElement("li");
                    li.classList.add("list-group-item", "cursor-pointer");
                    li.textContent = `${item.clinicName} - Clinic`;

                    Object.assign(li.dataset, {
                        regionId: item.regionid,
                        regionName: item.regionname,
                        provinceId: item.provinceId,
                        provinceName: item.provinceName,
                        municipalityId: item.municipalityId,
                        municipalityName: item.municipalityName,
                        clinicId: item.id,
                        clinicName: item.clinicName,
                    });

                    li.addEventListener("click", () => {
                        const clinicId = li.dataset.clinicId;
                        const municipalityId = li.dataset.municipalityId;

                        resultsList.style.display = "none";

                        if (clinicId && municipalityId) {
                            window.location.href = `/Home/ClinicDoctorsDetail?clinicId=${encodeURIComponent(clinicId)}&municipalityId=${encodeURIComponent(municipalityId)}`;
                        }
                    });

                    resultsList.appendChild(li);
                });

                resultsList.style.display = "block";
            })
            .catch(error => {
                console.error("Autocomplete error:", error);
            });
    });

    // Hide dropdown if clicking outside the input or list
    document.addEventListener("click", function (e) {
        if (!searchInput.contains(e.target) && !resultsList.contains(e.target)) {
            resultsList.style.display = "none";
        }
    });

    // Optional: Dropdown population helper
    function fillSelect(select, value, label) {
        let option = select.querySelector(`option[value='${value}']`);
        if (!option) {
            option = document.createElement("option");
            option.value = value;
            option.text = label;
            select.appendChild(option);
        }
        select.value = value;
    }
});
