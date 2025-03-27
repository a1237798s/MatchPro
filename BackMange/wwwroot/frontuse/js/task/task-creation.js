document.addEventListener("DOMContentLoaded", function () {
    // 進度指示器
    const progressSteps = document.querySelectorAll(".progress-step");
    const progressFill = document.querySelector(".progress-fill");
    let currentStep = 0;

    function updateProgress(step) {
        progressSteps.forEach((step, index) => {
            step.classList.remove("active");
            if (index <= currentStep) {
                step.classList.add("active");
            }
        });
        progressFill.style.width = `${(currentStep / (progressSteps.length - 1)) * 100
            }%`;
    }

    // 技能
    const skillInput = document.querySelector(".skills-input input");
    const skillsContainer = document.querySelector(".skills-tags");
    const skills = new Set();

    skillInput.addEventListener("keypress", function (e) {
        if (e.key === "Enter" && this.value.trim()) {
            e.preventDefault();
            const skill = this.value.trim();
            if (!skills.has(skill)) {
                skills.add(skill);
                addSkillTag(skill);
            }
            this.value = "";
        }
    });

    function addSkillTag(skill) {
        const tag = document.createElement("div");
        tag.className = "skill-tag";
        tag.innerHTML = `
            ${skill}
            <button type="button" onclick="removeSkill(this, '${skill}')">
                <i class="fas fa-times"></i>
            </button>
        `;
        skillsContainer.appendChild(tag);
    }

    window.removeSkill = function (button, skill) {
        skills.delete(skill);
        button.closest(".skill-tag").remove();
    };

    // 檔案上傳
    const fileUpload = document.querySelector(".file-upload");
    const fileInput = fileUpload.querySelector('input[type="file"]');
    const previewContainer = document.querySelector(".image-preview");

    fileUpload.addEventListener("dragover", function (e) {
        e.preventDefault();
        this.classList.add("dragover");
    });

    fileUpload.addEventListener("dragleave", function (e) {
        e.preventDefault();
        this.classList.remove("dragover");
    });

    fileUpload.addEventListener("drop", function (e) {
        e.preventDefault();
        this.classList.remove("dragover");
        const files = e.dataTransfer.files;
        handleFiles(files);
    });

    fileInput.addEventListener("change", function (e) {
        handleFiles(this.files);
    });

    function handleFiles(files) {
        Array.from(files).forEach((file) => {
            if (!file.type.startsWith("image/")) return;

            const reader = new FileReader();
            reader.onload = function (e) {
                addImagePreview(e.target.result);
            };
            reader.readAsDataURL(file);
        });
    }

    function addImagePreview(src) {
        const preview = document.createElement("div");
        preview.className = "preview-item";
        preview.innerHTML = `
            <img src="${src}" alt="預覽圖片">
            <button type="button" class="remove-image" onclick="removeImage(this)">
                <i class="fas fa-times"></i>
            </button>
        `;
        previewContainer.appendChild(preview);
    }

    window.removeImage = function (button) {
        button.closest(".preview-item").remove();
    };

    // 表單驗證
    const form = document.querySelector(".task-form");

    form.addEventListener("submit", function (e) {
        e.preventDefault();
        if (validateForm()) {
            submitForm();
        }
    });

    function validateForm() {
        let isValid = true;
        const requiredFields = form.querySelectorAll("[required]");

        requiredFields.forEach((field) => {
            const formGroup = field.closest(".form-group");
            if (!field.value.trim()) {
                isValid = false;
                formGroup.classList.add("error");
                if (!formGroup.querySelector(".error-message")) {
                    const error = document.createElement("div");
                    error.className = "error-message";
                    error.textContent = "此欄位為必填";
                    formGroup.appendChild(error);
                }
            } else {
                formGroup.classList.remove("error");
                const error = formGroup.querySelector(".error-message");
                if (error) error.remove();
            }
        });

        return isValid;
    }

    function submitForm() {
        // 收集表單數據
        const formData = new FormData(form);
        formData.append("skills", Array.from(skills));

        // 添加提交邏輯
        console.log("表單提交", Object.fromEntries(formData));

        // 提交成功 騙人的
        showSuccessMessage();
    }

    function showSuccessMessage() {
        const message = document.createElement("div");
        message.className = "success-message";
        message.innerHTML = `
            <i class="fas fa-check-circle"></i>
            任務已成功發布！
        `;
        form.insertAdjacentElement("beforebegin", message);

        setTimeout(() => {
            message.remove();
            // 重置表單
            form.reset();
            skillsContainer.innerHTML = "";
            skills.clear();
            previewContainer.innerHTML = "";
            currentStep = 0;
            updateProgress();
        }, 3000);
    }

    // 日期驗證
    const startDate = document.querySelector(
        'input[type="date"][name="start_date"]'
    );
    const endDate = document.querySelector('input[type="date"][name="end_date"]');

    if (startDate && endDate) {
        startDate.addEventListener("change", function () {
            endDate.min = this.value;
        });

        endDate.addEventListener("change", function () {
            startDate.max = this.value;
        });
    }

    // 預算輸入驗證
    const budgetInput = document.querySelector(
        'input[type="number"][name="budget"]'
    );
    if (budgetInput) {
        budgetInput.addEventListener("input", function () {
            if (this.value < 0) this.value = 0;
        });
    }
});
