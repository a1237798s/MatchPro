document.addEventListener("DOMContentLoaded", () => {

    const sliders = document.querySelectorAll(".image-slider");

    sliders.forEach((slider) => {
        const sliderContainer = slider.querySelector(".slider-container");
       
        const prevBtn = slider.querySelector(".slider-btn.prev");
        const nextBtn = slider.querySelector(".slider-btn.next");

        let currentSlide = 0;

        function goToSlide(index) {
            currentSlide = (index + images.length) % images.length;
            sliderContainer.style.transform = `translateX(-${currentSlide * 100}%)`;
        }

        prevBtn.addEventListener("click", (e) => {
            e.preventDefault(); // 阻止默認行為
            e.stopPropagation(); // 防止事件冒泡
            goToSlide(currentSlide - 1);
        });

        nextBtn.addEventListener("click", (e) => {
            e.preventDefault(); // 阻止默認行為
            e.stopPropagation(); // 防止事件冒泡
            goToSlide(currentSlide + 1);
        });
    });


    const sortDropdown = document.getElementById("sortOrder");

    // 排序功能
    if (sortDropdown) {
        sortDropdown.addEventListener("change", () => {
            const sortOrder = sortDropdown.value;
            // 重新加載排序
            window.location.href = `/Frontend/missionList2?sortOrder=${sortOrder}`;
        });
    }

    const template = document.getElementById("service-card-template");
    const container = document.querySelector(".services-container");

    // 假設從後端傳來的 `services` 資料結構包含多張圖片的 `images` 屬性
    services.forEach((service) => {
        const clone = template.content.cloneNode(true);

        // 圖片輪播
        const sliderContainer = clone.querySelector(".slider-container");
        const dotsContainer = clone.querySelector(".slider-dots");

        if (service.images && service.images.length > 0) {
            service.images.forEach((imgSrc, index) => {
                const img = document.createElement("img");
                img.src = imgSrc; // 從後端資料中獲取圖片路徑
                img.alt = service.title;
                img.style.width = "100%";
                img.style.objectFit = "cover";
                sliderContainer.appendChild(img);

                const dot = document.createElement("div");
                dot.className = `dot ${index === 0 ? "active" : ""}`;
                dot.addEventListener("click", () => {
                    goToSlide(index);
                });
                dotsContainer.appendChild(dot);
            });

            // 輪播邏輯
            let currentSlide = 0;
            const slides = sliderContainer.querySelectorAll("img");          
            const dots = dotsContainer.querySelectorAll(".dot");

            function goToSlide(n) {
                currentSlide = n;
                sliderContainer.style.transform = `translateX(-${n * 100}%)`;
                dots.forEach((dot, i) => {
                    dot.classList.toggle("active", i === n);
                });
            }

            clone.querySelector(".prev").addEventListener("click", (e) => {
                e.preventDefault();
                e.stopPropagation();
                currentSlide = (currentSlide - 1 + slides.length) % slides.length;
                goToSlide(currentSlide);
            });

            clone.querySelector(".next").addEventListener("click", (e) => {
                e.preventDefault();
                e.stopPropagation();
                currentSlide = (currentSlide + 1) % slides.length;
                goToSlide(currentSlide);
            });
        } else {
            // 如果沒有圖片，顯示預設圖片
            const placeholder = document.createElement("div");
            placeholder.textContent = "無圖片";
            placeholder.style.textAlign = "center";
            sliderContainer.appendChild(placeholder);
        }

        clone.querySelector(".avatar").src = service.providerAvatar;
        clone.querySelector(".avatar").alt = `${service.providerName} 的頭像`;
        clone.querySelector(".provider-name").textContent = service.providerName;

        clone.querySelector(".service-title").textContent = service.title;
        clone.querySelector(
            ".amount"
        ).textContent = `TWD ${service.price.toLocaleString()}`;

        const cardLink = clone.querySelector(".card-link");
        cardLink.href = `./service-detail.html?id=${service.id}`;

        const heartBtn = clone.querySelector(".heart-btn");
        heartBtn.addEventListener("click", (e) => {
            e.preventDefault();
            e.stopPropagation();
            const heartIcon = heartBtn.querySelector("i");
            heartIcon.classList.toggle("bi-heart");
            heartIcon.classList.toggle("bi-heart-fill");
            heartBtn.classList.toggle("active");
        });

        container.appendChild(clone);
    });

   
   


});

document.addEventListener("DOMContentLoaded", function () {
    const sortDropdown = document.getElementById("sortOrder");

    if (sortDropdown) {
        sortDropdown.addEventListener("change", function () {
            const selectedValue = sortDropdown.value;
            const currentUrl = new URL(window.location.href);

            // 設置 `sortOrder` 參數到 URL
            currentUrl.searchParams.set("sortOrder", selectedValue);

            // 重新導向新的 URL，讓 ASP.NET MVC 重新獲取排序後的資料
            window.location.href = currentUrl.toString();
        });
    }
});