const Message = {
    name: 'Message',
    setup() {
        const { ref, onMounted, onBeforeMount, watch, computed, inject, nextTick } = Vue;
        const user = inject("User");
        const isUnread = ref(true);
        // 定義響應式數據
        const proposalMessages = ref([]);
        const caseMessages = ref([]);
        const chatMsgDiv = ref(null);
        const newMessage = ref('');
        const activeType = ref('proposal');
        const activeMessageId = ref(null);
        const currentChat = ref({
            name: '',
            avatar: '',
            messages: [] // 初始化為空陣列
        });
        const messages = computed(() => currentChat.value?.messages || []);
        const typeTabs = computed(() => [
            { type: 'proposal', label: '提案訊息' },
            { type: 'case', label: '接案訊息' },
        ]);

        let connection = null; // 新增 SignalR connection 變數

        // 使用 Map 來儲存每個聊天室的未讀狀態
        const unreadStates = ref(new Map());

        // 監聽 user 的變化
        watch(user, (newVal, oldVal) => {
            console.log('User 數據更新:', newVal);
        }, { immediate: true });

        onMounted(async () => {
            console.log("Message 組件已掛載");
            try {
                // 等待一小段時間確保用戶數據已載入
                await new Promise(resolve => setTimeout(resolve, 100));
                await handleTypeChange('proposal');
            } catch (error) {
                console.error("初始化載入失敗:", error);
            }
        });


        // 新增 SignalR 連線方法
        async function connectToSignalR(userId, posterId) {
            console.log('正在連接到聊天室...', "workerId:", userId, "posterId:", posterId);

            connection = new signalR.HubConnectionBuilder()
                .withUrl(`https://localhost:7253/chatSr?UserId=${userId}&PosterId=${posterId}`)
                .build();

            try {
                await connection.start();
                console.log("Hub 連線完成");

                // 註冊更新聊天內容事件
                connection.on("UpdContent", (msg) => {

                    if (msg.user == user.value.FuserId) {
                        currentChat.value.messages.push({
                            Type: "sent",
                            Content: msg.message,
                            Time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
                        });

                    } else if (msg.user != user.value.FuserId && msg.user != null) {
                        currentChat.value.messages.push({
                            Type: "received",
                            Content: msg.message,
                            Time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
                        });
                    }
                    console.log("個頁更新訊息", currentChat.value, messages.value)
                    nextTick(() => {
                        scrollToBottom();
                    });
                });

            } catch (err) {
                console.error("連線錯誤:", err);
                alert("連線錯誤: " + err.toString());
            }
        }



        async function fetchMessages(type) {
            try {

                // 檢查用戶ID是否存在
                if (!user.value?.FuserId) {
                    console.warn("用戶ID不存在，等待用戶資料載入");
                    return;
                }

                // console.log(`fetchMessages發送請求: ${type}`);
                const response = await fetch('/api/ShowMsg/ListMsg', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        msgType: type,
                        userId: user.value.FuserId
                    })
                });

                if (!response.ok) {
                    throw new Error('網路回應不正常');
                }

                const data = await response.json();

                console.log("fetchMessages 回傳物件:", data.Messages);

                // 排序函數
                const sortByTime = (messages) => {
                    return messages
                        .map(message => ({
                            ...message,
                            timestamp: message.Time ? new Date(message.Time) : new Date(0)
                        }))
                        .sort((a, b) => a.timestamp - b.timestamp); // new to old
                };

                //根據類型更新對應的訊息列表
                if (type === 'Post') {
                    proposalMessages.value = data.Messages
                        .map(message => ({
                            id: message.ChatId,
                            userName: message.UserName,
                            avatar: message.Avatar,
                            lastMessage: message.LastMessage,
                            time: message.Time ? (() => {
                                const date = new Date(message.Time);
                                const year = String(date.getFullYear()).slice(-2);
                                const month = String(date.getMonth() + 1).padStart(2, '0');
                                const day = String(date.getDate()).padStart(2, '0');
                                const hours = String(date.getHours()).padStart(2, '0');
                                const minutes = String(date.getMinutes()).padStart(2, '0');
                                return `${year}/${month}/${day} ${hours}:${minutes}`;
                            })() : "00/00/00 00:00",
                            timestamp: message.Time ? new Date(message.Time).getTime() : 0, // 使用毫秒時間戳記來排序
                            unread: message.Unread,
                            type: "proposal",
                            status: message.Status,
                            proposalStatus: "pending",
                            allMessages: message.AllMessages
                        }))
                        .sort((a, b) => b.timestamp - a.timestamp); // 降序排列（新到舊）

                    console.log("排序後的提案訊息", proposalMessages.value);
                } else if (type === 'Work') {
                  caseMessages.value = data.Messages
                  .map(message => ({
                      id: message.ChatId,
                      userName: message.UserName,
                      avatar: message.Avatar,
                      lastMessage: message.LastMessage,
                      time: message.Time ? (() => {
                          const date = new Date(message.Time);
                          const year = String(date.getFullYear()).slice(-2);
                          const month = String(date.getMonth() + 1).padStart(2, '0');
                          const day = String(date.getDate()).padStart(2, '0');
                          const hours = String(date.getHours()).padStart(2, '0');
                          const minutes = String(date.getMinutes()).padStart(2, '0');
                          return `${year}/${month}/${day} ${hours}:${minutes}`;
                      })() : "00/00/00 00:00",
                      timestamp: message.Time ? new Date(message.Time).getTime() : 0, // 使用毫秒時間戳記來排序
                      unread: message.Unread,
                      type: "case",
                      status: message.Status,
                      allMessages: message.AllMessages
                  }))
                  .sort((a, b) => b.timestamp - a.timestamp);
                }

                //console.log(`fetchMessages發送請求: ${type}`);
                //const response = await fetch('/api/ShowMsg/ListMsg', {
                //    method: 'POST',
                //    headers: { 'Content-Type': 'application/json' },
                //    body: JSON.stringify({ Type: type }) 
                //});
                //console.log("回應:", response);
                //const data = await response.json();
                //console.log('回應結果:', data);
            }
            catch (error) {
                console.error('獲取訊息失敗:', error);
            }
        }

        // 切換訊息類型時重新獲取數據
        async function setActiveType(type) {
            activeType.value = type;
            console.log(`setActiveType: ${type}`);
            await fetchMessages(type === 'proposal' ? 'Post' : 'Work');
        }

        async function handleChatChange(chatId, type) {
            activeMessageId.value = chatId;
            unreadStates.value.set(chatId, false);
            let chat;
            let chatIdpMsgs = proposalMessages.value.find(message => message.id === chatId);
            let chatIdcMsgs = caseMessages.value.find(message => message.id === chatId);
            let otherId;

            // 標記訊息為已讀
            try {
                const response = await fetch(`/api/ShowMsg/MarkAsRead/${chatId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        userId: user.value.FuserId
                    })
                });

                if (response.ok) {
                    const data = await response.json();
                    if (data.success) {
                        console.log('訊息已標記為已讀');
                        // 清除未讀訊息數
                        if (type === 'proposal' && chatIdpMsgs) {
                            chatIdpMsgs.unread = 0;
                        } else if (type === 'case' && chatIdcMsgs) {
                            chatIdcMsgs.unread = 0;
                        }
                    } else {
                        console.error('標記已讀失敗:', data.message);
                    }
                }
            } catch (error) {
                console.error('標記已讀時發生錯誤:', error);
            }

            if (connection) {
                try {
                    await connection.stop();
                    console.log("已結束舊的聊天室連線");
                } catch (err) {
                    console.error("結束舊連線時發生錯誤:", err);
                }
                connection = null;
            }

            if (type === 'proposal') {
                
                chat = chatIdpMsgs.allMessages;
                currentChat.value.name = chatIdpMsgs.userName;
                currentChat.value.avatar = chatIdpMsgs.avatar;
                console.log("currentChat.value.avatar", currentChat.value.avatar)
            } else if (type === 'case') {
                
                chat = chatIdcMsgs.allMessages;
                currentChat.value.name = chatIdcMsgs.userName;
                currentChat.value.avatar = chatIdcMsgs.avatar;
                console.log("currentChat.value.avatar", currentChat.value.avatar)
            } else {
                console.log("沒抓到聊天訊息!!");
                return;
            }

            // 找出對方的 ID（第一個不等於當前用戶的 SenderId）
            otherId = chat.find(msg => msg.SenderId !== user.value.FuserId)?.SenderId;
            console.log("找到對方ID:", otherId);
            // 確保 user 數據存在且有 FuserId
            if (user.value?.FuserId) {
                // 如果連線不存在或已斷開，則建立新連線
                if (!connection || connection.state !== "Connected") {
                    if (type === 'proposal') {
                        await connectToSignalR(otherId, user.value.FuserId);
                    } else if (type === 'case') {
                        await connectToSignalR(user.value.FuserId, otherId);
                    }
                }
            } else {
                console.error('無法建立連線');
                return;
            }


            if (chat) {
                currentChat.value = {
                    name: currentChat.value.name,
                    avatar: currentChat.value.avatar,
                    messages: chat.map(msg => ({
                        ...msg, // 保留其他屬性
                        Time: new Date(msg.Time).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) // 格式化時間
                    }))
                }; // 設置當前聊天
                console.log("當前聊天內容:", currentChat.value);
            } else {
                console.log("找不到對應的聊天訊息!!");
            }
        }

        async function handleTypeChange(type) {
            console.log("handleTypeChange:", type)
            activeType.value = type;
            await fetchMessages(type === 'proposal' ? 'Post' : 'Work');
            autoSelectFirstChat();
        }

        // 自動選擇第一個聊天室
        function autoSelectFirstChat() {
            console.log("自動選擇第一個聊天室");
            try {
                // 根據當前 activeType 選擇對應的消息列表
                const currentMessages = activeType.value === 'proposal' ?
                    proposalMessages.value : caseMessages.value;

                // 檢查是否有聊天記錄
                if (currentMessages && currentMessages.length > 0) {
                    const firstChat = currentMessages[0];
                    activeMessageId.value = firstChat.id;
                    console.log("找到第一個聊天室:", firstChat);

                    // 調用現有的 handleChatChange 方法來開啟聊天室
                    handleChatChange(
                        firstChat.id,
                        activeType.value
                    );
                } else {
                    console.log("沒有可用的聊天室");
                }
            } catch (error) {
                console.error("自動選擇聊天室時發生錯誤:", error);
            }
        }

        // 發送消息
        async function sendMessage() {
            console.log("currentChat:", currentChat.value);
            console.log("newMessage:", newMessage.value);
            if (!newMessage.value || !currentChat.value) return;

            const now = new Date();
            const Time = `${now.getHours() >= 12 ? '下午' : '上午'} ${now.getHours() % 12 || 12}:${String(now.getMinutes()).padStart(2, '0')}`;
            const message = { Type: 'sent', Content: newMessage.value, Time };

            // 將新消息添加到聊天數據和當前聊天中
            //currentChat.value.messages.push(message);
            try {
                await connection.invoke("SendMessage", user.value.FuserId, newMessage.value);
            } catch (err) {
                alert("傳送錯誤: " + err.toString());
            }
            newMessage.value = ''; // 清空輸入框
            // 滾動到底部
            await nextTick();
            scrollToBottom();
        }

        function scrollToBottom() {
            const container = chatMsgDiv.value;
            if (container) {
                container.scrollTop = container.scrollHeight;
                //console.log("scrollToBottom!",container);

            }
        }

        // 處理鍵盤按鍵事件
        function handleKeyPress(e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                console.log("Enter:", newMessage);
                e.preventDefault(); // 防止換行
                sendMessage(); // 發送消息
            }
        }

        // 在接收新訊息的處理函數中
        function handleNewMessage(message, chatId) {
            // ... 原有的處理邏輯 ...
            // 設置特定聊天室的未讀狀態為 true
            unreadStates.value.set(chatId, true);
        }

        return {
            activeMessageId,
            activeType,
            caseMessages,
            currentChat,
            chatMsgDiv,
            messages,
            newMessage,
            proposalMessages,
            typeTabs,
            isUnread,
            unreadStates,
            handleChatChange,
            handleKeyPress,
            handleTypeChange,
            setActiveType,
            sendMessage,

        };
    },
    template: `
<div class="message-interface">
  <!-- 左側訊息列表 -->
  <div class="message-list-container">
    <!-- msg-tab -->
    <div class="message-tabs">
      <button
        v-for="tab in typeTabs"
        :key="tab.type"
        :class="['type-tab', { active: activeType === tab.type }]"
        @click="handleTypeChange(tab.type)"
      >
        {{ tab.label }}
      </button>
    </div>

    <!-- 訊息列表容器 -->
    <!-- 提案列表 -->
    <div :class="['chat-list', {active: activeType ==='proposal'}]">
      <div
        v-for="message in proposalMessages"
        :key="message.id"
        :class="['chat-item', { active: activeMessageId === message.id }]"
        @click="handleChatChange(message.id, 'proposal')"
      >
        <div class="chat-item-left">
          <div class="chat-avatar">
            <img :src="message.avatar" :alt="message.userName" />
          </div>
        </div>
        <div class="chat-item-right">
          <div class="chat-item-header">
            <span class="chat-item-name">{{message.userName}}</span>
            <span class="chat-item-time">{{ message.time }}</span>
          </div>
          <div class="chat-item-message">{{ message.lastMessage }}</div>
          <div class="unread-badge" v-show="(message.unread != 0)">
            {{message.unread}}
          </div>
        </div>
      </div>
    </div>
    <!-- 接案列表 -->
    <div :class="['chat-list', {active: activeType ==='case'}]">
      <div
        v-for="message in caseMessages"
        :key="message.id"
        :class="['chat-item', { active: activeMessageId === message.id }]"
        @click="handleChatChange(message.id, 'case')"
      >
        <div class="chat-item-left">
          <div class="chat-avatar">
            <img :src="message.avatar" :alt="message.userName" />
          </div>
        </div>
        <div class="chat-item-right">
          <div class="chat-item-header">
            <span class="chat-item-name">{{message.userName}}</span>
            <span class="chat-item-time">{{ message.time }}</span>
          </div>
          <div class="chat-item-message">{{ message.lastMessage }}</div>
          <div class="unread-badge" v-show="(message.unread != 0)">
            {{message.unread}}
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- 右側聊天視窗 -->
  <div class="chat-view">
    <!-- 聊天室頂部 -->
    <div class="chat-header">
      <div class="chat-title">
        <!-- 左側用戶資訊 -->
        <div class="chat-user-info">
          <img
            :src="currentChat.avatar"
            :alt="currentChat.name"
            class="chat-user-avatar"
          />
          <span class="chat-user-name">{{currentChat.name}}</span>
        </div>

        <!-- 右側專案資訊 -->
        <div class="chat-user-details">
          <!-- <div class="project-info">
            <span class="project-name">未知專案</span>
            <span class="project-status 進行中">進行中</span>
          </div> -->
        </div>
      </div>
    </div>

    <!-- 聊天訊息區域 -->
    <div class="chat-messages" ref="chatMsgDiv">
      <div
        v-for="msg in messages"
        :key="msg.Time"
        class="message-bubble"
        :class="msg.Type"
      >
        <div class="message-content-wrapper">
          <div class="message-content">{{ msg.Content }}</div>
          <span class="message-time">{{ msg.Time }}</span>
        </div>
      </div>
    </div>

    <!-- 輸入區域 -->
    <div class="chat-input-area">
      <div class="input-wrapper">
        <textarea
          v-model="newMessage"
          @keydown="handleKeyPress"
          class="chat-input"
          placeholder="輸入訊息..."
          rows="1"
        ></textarea>
        <button class="send-btn" @click="sendMessage">
          <i class="bi bi-send-fill"></i>
        </button>
      </div>
    </div>
  </div>
</div>
`

};
// 輔助函數：獲取當前用戶ID
function getUserId() {
    // 這裡需要實現獲取當前用戶ID的邏輯
    // 可能從 localStorage、cookie 或其他地方獲取
    return 3; // 暫時返回固定值
}

// 將組件掛載到全局變數
window.MessageComponent = Message;