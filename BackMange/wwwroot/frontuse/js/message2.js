const Message = {
    name: 'Message',
    setup() {
        const { ref, onMounted } = Vue;

        // 定義響應式數據
        const proposalMessages = ref([]);
        const caseMessages = ref([]);
        const activeType = ref('proposal');

        async function fetchMessages(type) {
            try
            {
                console.log(`發送請求: ${type}`);
                const response = await fetch('/Messages/ListMsg', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ Type: type }) // 傳送 JSON 對象
                });

                const contentType = response.headers.get("content-type");
                if (!contentType || !contentType.includes("application/json")) {
                    const text = await response.text();
                    throw new Error(`伺服器返回非 JSON 格式: ${text}`);
                }

                const data = await response.json();
                console.log('回應結果:', data);
            }
            catch (error)
            {
                console.error('請求失敗:', error);
            }   
        }        
        
        // 切換訊息類型時重新獲取數據
        async function setActiveType(type) {
            activeType.value = type;  
            console.log(`正在獲取訊息類型: ${type}`);   
            //await fetchMessages(type == 'proposal' ? 'Post' : 'Work');
            await fetchMessages("Post");
        }
        
        return {
            proposalMessages,
            caseMessages,
            activeType,
            setActiveType,
            fetchMessages
        };
    },
    template: `
        <div class="message-interface">
            <div class="message-list-container">
                <div class="page-header">
                    <h2>訊息管理</h2>
                </div>
                <!-- 訊息類型切換 -->
                <div class="message-tabs">
                    <button 
                        :class="['type-tab', { active: activeType === 'proposal' }]"
                        @click="setActiveType('proposal')"
                    >
                        提案訊息
                        <span v-if="proposalUnread > 0" class="badge bg-danger">{{ proposalUnread }}</span>
                    </button>
                    <button 
                        :class="['type-tab', { active: activeType === 'case' }]"
                        @click="setActiveType('case')"
                    >
                        接案訊息
                        <span v-if="caseUnread > 0" class="badge bg-danger">{{ caseUnread }}</span>
                    </button>
                </div>

                <!-- 提案訊息列表 -->
                <div v-show="activeType === 'proposal'" class="message-list">
                    <div v-for="message in proposalMessages" 
                         :key="message.id" 
                         class="message-card"
                         @click="openChat(message)">
                        <div class="message-header">
                            <div class="user-info">
                                <img :src="message.avatar" :alt="message.userName">
                                <div class="user-details">
                                    <div class="user-name">{{ message.userName }}</div>
                                    <div class="project-name">{{ message.projectName }}</div>
                                </div>
                            </div>
                            <div class="message-time">{{ message.time }}</div>
                        </div>
                        <div class="message-content">{{ message.lastMessage }}</div>
                        <div class="message-footer">
                            <div :class="['read-status', { unread: message.unread > 0 }]">
                                <i :class="message.unread > 0 ? 'bi bi-envelope-fill' : 'bi bi-envelope-open'"></i>
                                {{ message.unread > 0 ? message.unread + ' 則未讀' : '已讀' }}
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 接案訊息列表 -->
                <div v-show="activeType === 'case'" class="message-list">
                    <div v-for="message in caseMessages" 
                         :key="message.id" 
                         class="message-card"
                         @click="openChat(message)">
                        <!-- 與提案訊息相同的結構 -->
                    </div>
                </div>
            </div>
        </div>
    `
};

// 將組件掛載到全局變數
window.MessageComponent = Message;