const Login = {
  data() {
    return {
      isLogin: true,
    };
  },
  template: `
    <section class="login-section">
      <div class="login-container">
        <div class="login-left">
          <img src="./img/lighthouse.jpg" alt="Lighthouse" class="login-image">
        </div>
        <div class="login-right">
          <div class="login-content">
            <h1 class="login-title">MatchPro</h1>
            <p class="login-subtitle">探索屬於你的精彩機會</p>

            <div class="login-tabs">
              <button 
                :class="['tab-btn', { active: isLogin }]"
                @click="isLogin = true"
              >登入</button>
            
            </div>

            <!-- 登入表單 -->
            <form v-if="isLogin" class="login-form">
              <div class="form-group">
                <input type="email" class="form-control" placeholder="電子信箱">
              </div>
              <div class="form-group">
                <input type="password" class="form-control" placeholder="密碼">
              </div>
              
              <div class="social-login">
                 <button type="submit" class="submit-btn">登入</button>
      
              
              </div>

              <div class="divider">
                <span>或</span>
              </div>
  <button type="button" class="social-btn google">
                  <i class="bi bi-google"></i>
                  <span>Google</span>
                </button>
           
            </form>

            <!-- 註冊表單 -->
            <form v-else class="login-form">
              <div class="form-group">
                <input type="email" class="form-control" placeholder="電子信箱">
              </div>
              <div class="form-group">
                <input type="password" class="form-control" placeholder="密碼">
              </div>
              <div class="form-check">
                <input type="checkbox" class="form-check-input" id="terms">
                <label class="form-check-label" for="terms">
                  我同意 <a href="#">服務條款</a> 和 <a href="#">隱私政策</a>
                </label>
              </div>
              <button type="submit" class="submit-btn">註冊</button>
            </form>

          </div>
        </div>
      </div>
    </section>
  `,
};
