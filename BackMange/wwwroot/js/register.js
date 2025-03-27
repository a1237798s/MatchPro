
    ////document.addEventListener("DOMContentLoaded", function () {
    ////              const birthdateInput = document.getElementById("birthdate");
    ////const today = new Date();
    ////const maxDate = today.toISOString().split("T")[0];
    ////birthdateInput.max = maxDate;

    ////              /* document.getElementById('lastName').value = 'Name1';
    ////              document.getElementById('firstName').value = 'Name2';
    ////              document.getElementById('birthdate').value = '1998-09-01';
    ////              document.getElementById('gender').value = 'male';
    ////              document.getElementById('email').value = '123@gmail.com';
    ////              document.getElementById('idNumber').value = 'T123456789';
    ////              document.getElementById('password').value = 'Aa123';
    ////              document.getElementById('confirmPassword').value = 'Aa123'; */
    ////            });

    ////// 用來控制步驟顯示與切換
    ////function showStep(stepNumber) {
    ////    document.querySelectorAll('.step').forEach(function (step) {
    ////        step.classList.remove('active');
    ////    });
    ////document.getElementById('step' + stepNumber).classList.add('active');
    ////// 可加入簡單進入動畫（依照原本的 .enter 樣式）
    ////document.querySelector('.register-box').classList.remove('enter');
    ////              setTimeout(() => {
    ////    document.querySelector('.register-box').classList.add('enter');
    ////              }, 10);
    ////            }

    ////// 初始顯示第一步
    ////showStep(1);

    ////// Step 1：輸入姓名
    ////document.getElementById('btnNext1').addEventListener('click', function () {
    ////              var lastName = document.getElementById('lastName').value;
    ////var firstName = document.getElementById('firstName').value;

    ////if (!firstName) {
    ////    alert('請輸入名字');
    ////return;
    ////              }

    ////// 如有需要可將資料存入 localStorage
    ////localStorage.setItem('lastName', lastName);
    ////localStorage.setItem('firstName', firstName);

    ////showStep(2);
    ////            });

    ////// Step 2：基本資訊
    ////document.getElementById('btnBack2').addEventListener('click', function () {
    ////    showStep(1);
    ////            });
    ////document.getElementById('btnNext2').addEventListener('click', function () {
    ////              var birthdate = document.getElementById('birthdate').value;
    ////var gender = document.getElementById('gender').value;

    ////if (!birthdate || !gender) {
    ////    alert('請填寫所有必填欄位');
    ////return;
    ////              }

    ////// 18歲以下禁止註冊
    ////const birthdateObj = new Date(birthdate);
    ////const age = calculateAge(birthdateObj);

    ////if (age < 18) {
    ////    alert("未滿18歲禁止註冊");
    ////return;
    ////              }

    ////localStorage.setItem('birthdate', birthdate);
    ////localStorage.setItem('gender', gender);
    ////showStep(3);
    ////            });

    ////// Step 3：設定帳號（頭貼、電子郵件與身分證字號）
    ////// 處理頭貼上傳
    ////var photoInput = document.getElementById('photoInput');
    ////var photoPreview = document.getElementById('photoPreview');
    ////var uploadBtn = document.querySelector('.btn-upload');
    ////var deletePhotoBtn = document.querySelector('.delete-photo');

    ////// 點擊頭貼預覽區就開啟檔案選擇
    ////photoPreview.parentElement.addEventListener('click', function (e) {
    ////              if (!e.target.closest('.delete-photo')) {
    ////    photoInput.click();
    ////              }
    ////            });

    ////uploadBtn.addEventListener('click', function (e) {
    ////    e.preventDefault();
    ////if (uploadBtn.classList.contains('cancel')) {
    ////    resetPhotoUpload();
    ////              } else {
    ////    photoInput.click();
    ////              }
    ////            });

    ////photoInput.addEventListener('change', function (e) {
    ////              var file = e.target.files[0];
    ////if (file) {
    ////                if (file.size > 5 * 1024 * 1024) {
    ////    alert('圖片大小不能超過 5MB');
    ////return;
    ////                }
    ////if (!file.type.startsWith('image/')) {
    ////    alert('請上傳圖片檔案');
    ////return;
    ////                }
    ////var reader = new FileReader();
    ////reader.onload = function (e) {
    ////    photoPreview.src = e.target.result;
    ////localStorage.setItem('userPhoto', e.target.result);
    ////deletePhotoBtn.style.display = 'flex';
    ////uploadBtn.textContent = '取消上傳';
    ////uploadBtn.classList.add('cancel');
    ////                };
    ////reader.readAsDataURL(file);
    ////              }
    ////            });

    ////deletePhotoBtn.addEventListener('click', function (e) {
    ////    e.stopPropagation();
    ////resetPhotoUpload();
    ////            });

    ////function resetPhotoUpload() {
    ////    photoPreview.src =
    ////    'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSDq8CzwzyMdSJcr0vGfI5CGxksfWC-nKYKvw&s';
    ////localStorage.removeItem('userPhoto');
    ////deletePhotoBtn.style.display = 'none';
    ////photoInput.value = '';
    ////uploadBtn.textContent = '上傳頭貼';
    ////uploadBtn.classList.remove('cancel');
    ////            }

    ////// 如 localStorage 中已有頭貼資料則顯示
    ////var savedPhoto = localStorage.getItem('userPhoto');
    ////if (savedPhoto) {
    ////    photoPreview.src = savedPhoto;
    ////deletePhotoBtn.style.display = 'flex';
    ////uploadBtn.textContent = '上傳頭貼';
    ////uploadBtn.classList.add('cancel');
    ////            }

    ////// Step 3：帳號表單驗證與切換
    ////document.getElementById('btnBack3').addEventListener('click', function () {
    ////    showStep(2);
    ////            });
    ////document.getElementById('btnNext3').addEventListener('click', function () {
    ////              var email = document.getElementById('email').value;
    ////var idNumber = document.getElementById('idNumber').value;

    ////if (!email || !idNumber) {
    ////    alert('請填寫所有必填欄位');
    ////return;
    ////              }
    ////if (!validateEmail(email)) {
    ////    alert('請輸入有效的電子郵件地址');
    ////return;
    ////              }
    ////if (!validateIdNumber(idNumber)) {
    ////    alert('請輸入有效的身分證字號');
    ////return;
    ////              }

    ////localStorage.setItem('email', email);
    ////localStorage.setItem('idNumber', idNumber);
    ////showStep(4);
    ////            });

    ////function validateIdNumber(id) {
    ////              return /^[A-Za-z][1-2]\d{8}$/.test(id);
    ////            }
    ////function validateEmail(email) {

    ////}

    ////            // Step 4：設定密碼
    ////document.getElementById('btnBack4').addEventListener('click', function () {
    ////    showStep(3);
    ////            });
    ////document.getElementById('btnNext4').addEventListener('click', function () {
    ////              var password = document.getElementById('password').value;
    ////var confirmPassword = document.getElementById('confirmPassword').value;

    ////if (!password || !confirmPassword) {
    ////    alert('請填寫所有必填欄位');
    ////return;
    ////              }
    ////if (password !== confirmPassword) {
    ////    alert('密碼和確認密碼不一致');
    ////return;
    ////              }

    ////localStorage.setItem('password', password);
    ////// 完成註冊後，可進行表單送出或導向其他頁面
    ////var inputElements = document.querySelectorAll(
    ////'input[type="text"], input[type="email"], input[type="password"], input[type="date"]'
    ////);
    ////var arr = [];
    ////inputElements.forEach(function (input) {
    ////    // 將每個元素的 id 與 value 串成字串後推入陣列中
    ////    arr.push(input.id + ": " + input.value);
    ////              });
    ////alert(JSON.stringify(arr));
    ////alert('註冊成功！');
    ////              // 例如：window.location.href = 'welcome.html';
    ////            });

    ////function calculateAge(birthdate) {
    ////              const today = new Date();
    ////let age = today.getFullYear() - birthdate.getFullYear();
    ////const monthDiff = today.getMonth() - birthdate.getMonth();

    ////if (
    ////monthDiff < 0 ||
    ////(monthDiff === 0 && today.getDate() < birthdate.getDate())
    ////) {
    ////    age--;
    ////              }

    ////return age;
    ////            }


