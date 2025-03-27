//const { createApp, ref, onMounted, onUnmounted, provide, inject, computed } = Vue;
const chatApp = createApp({
    data() {
        return {
            isVisible: false, // room on
            isBubbleVisible: false, // room close
            isTyping: false,
            message: "",
            currentChat: {
                name: '',
                messages: [
                    //{
                    //    Type: "received",
                    //    Content: "測試顯示",
                    //    Time: null,
                    //}
                ]
            },
            thePosterId: window.POSTER_ID || window.WORKER_ID ||null,
            currentUserId: window.USER_ID,  //SR用
            connection: null,
            historyMsgs: [],
            // 智能回覆用
            //defaultReplies: {
            //    你好: "您好！很高興為您服務。",
            //    hi: "Hi！有什麼我可以幫您的嗎？",
            //    hello: "Hello！請問需要什麼協助呢？",
            //    謝謝: "不客氣！還有什麼我可以幫您的嗎？",
            //    掰掰: "再見！祝您有愉快的一天！",
            //    再見: "再見！如果還有問題歡迎隨時詢問。",
            //},
        };
    },

    created() {
        
        // 設置事件監聽器
        this.setupSignalRListeners();
    },

    watch: {
        thePosterId: {
            handler(newValue, oldValue) {
                console.log('thePosterId 變更:', oldValue, '->', newValue);
                // if (newValue && newValue !== oldValue) {
                //     // 當 ID 變更時，可以在這裡處理相關邏輯
                //     // 例如：重新建立聊天連接
                //     this.enterRoom(this.currentUserId);
                //     console.log('重新連接聊天室，案主ID:', newValue);
                // }
            },
            immediate: true  // 組件創建時立即執行一次
        },
        currentUserId: {
            handler(newValue, oldValue) {
                console.log('currentUserId 變更:', oldValue, '->', newValue);
             },
            immediate: true  // 組件創建時立即執行一次
        }

    },


    mounted() {
        //console.log("window.POSTER_ID mounted", window.POSTER_ID);
        //秒後顯示氣泡
        setTimeout(() => {
            if (!this.isVisible) {
                this.isBubbleVisible = true;
            }
            //console.log("isBubbleVisible", this.isBubbleVisible);
        }, 500);
        

        // 調用 enterRoom 方法，傳遞用戶 ID
        this.enterRoom(this.currentUserId, this.thePosterId);

        // 監聽點擊外部
        document.addEventListener("click", this.handleOutClickOrNot);

        // 初始化時獲取工作訊息
        this.fetchWorkMsg();
    },

    beforeUnmount() {
        document.removeEventListener("click", this.handleOutClickOrNot);
        // 清理 SignalR 連接
        if (this.connection) {
            this.connection.off("UpdContent", this.onUpdContent);
            this.connection.stop();
        }
    },

    computed: {
        messages() {
            return this.currentChat.messages;
        }
    },

    

    methods: {
        async fetchWorkMsg() {

            if (!this.currentUserId) {
                console.warn("用戶ID不存在，等待用戶資料載入");
                return;
            }
            console.log(`fetchMessages發送請求`);
            const response = await fetch('/api/ShowMsg/ListMsg', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    msgType: 'Work',
                    userId: this.currentUserId,
                    posterId: this.thePosterId
                })
            });            
            if (!response.ok) {
                throw new Error('網路回應不正常');
            }
            const data = await response.json();
            console.log("fetchWorkMsg 回傳物件:", data.Messages);
            
            this.historyMsgs = data.Messages.map(msg => msg.AllMessages);
            console.log("historyMsgs:", this.historyMsgs);

            if (this.historyMsgs && this.historyMsgs.length > 0) {
                // 修正：historyMsgs 是一個數組的數組，需要展平
                const flatMessages = this.historyMsgs.flat();
                
                this.currentChat = {
                    name: data.Messages[0].UserName, // 假設所有消息都來自同一個用戶
                    messages: flatMessages.map(msg => ({
                        Content: msg.Content,
                        Time: new Date(msg.Time).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
                        Type: msg.Type // 確保包含 Type 屬性
                    }))
                };
                
                console.log("當前聊天內容:", this.currentChat);
            } else {
                console.log("找不到對應的聊天訊息!!");
            }
        },

        setupSignalRListeners() {
            // 將事件監聽邏輯抽離出來
            this.onUpdContent = (msg) => {
                console.log("currentChat:", this.currentChat);
                console.log("監聽訊息:", msg);
                console.log("監聽msg_user:", msg.user, this.currentUserId);
                //console.log("條件if:", msg.user == this.currentUserId);
                //console.log("條件else if:", msg.user != this.currentUserId && msg.user != null);

                this.isTyping = false;
                if (msg.user == this.currentUserId) {
                    console.log("sent", msg.user == this.currentUserId)
                    this.addMessage({
                        type: "sent",
                        content: msg.message,
                        time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
                    });
                    // console.log("sent", this.currentChat.messages , this.messages);
                    console.log("麵包超人發送訊息")
                }
                else if (msg.user != this.currentUserId && msg.user != null) {
                    console.log("received", msg.user != this.currentUserId && msg.user != null)
                    this.addMessage({
                        type: "received",
                        content: msg.message,
                        time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
                    });
                    console.log("received", this.currentChat.messages, this.messages);
                    console.log("麵包超人收到訊息 發送人msg.user:", msg.user)
                    //this.messages.push({
                    //    type: "received",
                    //    content: msg.message,
                    //});
                }

                this.$nextTick(() => {
                    this.scrollToBottom();
                });
            };
        },

        enterRoom(userId, posterId) {
            console.log('右下enterRoom:', userId, posterId);
            // 建立新連接
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(
                    `https://localhost:7253/chatSr?UserId=${userId}&PosterId=${posterId}`
                )
                .build();
            // 建立連接
            this.connection
                .start()
                .then(() => {
                    console.log("Hub 連線完成");
                    // 連接成功後才註冊事件監聽
                    this.connection.on("UpdContent", this.onUpdContent);
                })
                .catch((err) => {
                    alert("連線錯誤: " + err.toString());
                });
        },

        closeChat() {
            this.isVisible = !this.isVisible;
            this.isBubbleVisible = !this.isBubbleVisible;
            // 關閉時重置所有狀態
            this.message = "";
            this.isTyping = false;
            console.log("closeChat");
        },

        handleOutClickOrNot(e) {
            const chatWidget = this.$refs.chatWidget; 
            const chatToggle = this.$refs.chatToggle; //右下圖示
            const chatBtn = document.getElementById('chatBtn'); // 聊聊 或 與我聯絡的按鈕
            //console.log(this.isVisible, chatWidget, !chatWidget.contains(e.target), !chatToggle.contains(e.target))
            if ( //點外部
                this.isVisible &&
                chatWidget &&
                !chatWidget.contains(e.target) &&
                !chatToggle.contains(e.target)
            ) {
                console.log("外部點擊關閉");
                this.closeChat();
            }
            else if ( //點擊 Icon
                chatToggle.contains(e.target) || chatBtn.contains(e.target)
            ) {
                    if (!this.isVisible) {
                    this.isVisible = !this.isVisible;
                    this.isBubbleVisible = !this.isBubbleVisible;
                    console.log(this.isVisible, 'Room on')
                    this.$nextTick(() => {
                        this.$refs.chatInput.focus();
                    });
                    return;
                }
                this.closeChat();
                console.log(this.isVisible, '透過Icon 關閉')
                //return
            }
            else {
                //console.log('無效點擊')
            }
        },

        getSmartReply(message) {
            const lowerMsg = message.toLowerCase();

            if (lowerMsg.includes("價格") || lowerMsg.includes("費用")) {
                return "關於價格方案，建議您可以參考我們的方案頁面，或是告訴我您的需求，我可以為您推薦最適合的方案。";
            }
            if (lowerMsg.includes("如何") || lowerMsg.includes("怎麼")) {
                return "需要更詳細了解您的問題，可以告訴我您想知道的具體內容嗎？";
            }
            if (lowerMsg.includes("聯絡") || lowerMsg.includes("客服")) {
                return "您可以撥打我們的服務專線09999999999999，或留下您的聯絡方式，我們會盡快與您聯繫。";
            }

            for (let key in this.defaultReplies) {
                if (lowerMsg.includes(key)) {
                    return this.defaultReplies[key];
                }
            }

            return "抱歉，我可能沒有完全理解您的問題。您可以換個方式描述，或直接聯繫我們的客服人員。";
        },

        async sendMessage() {
            if (!this.message.trim()) return;

            const user = Number(this.currentUserId);
            console.log("發送－user為", user)
            const userMessage = String(this.message);
            console.log("發送－msg為", userMessage)
            //// 添加用戶訊息
            //this.messages.push({
            //    type: "sent",
            //    content: this.message,
            //});

            // 傳送訊息
            try {
                await this.connection.invoke("SendMessage", user, userMessage);
            } catch (err) {
                console.error('發送訊息時發生錯誤:', err);
                alert(`傳送錯誤: ${err.message || err.toString()}`);
            }
            this.message = "";

            // 滾動到底部
            await this.$nextTick();
            this.scrollToBottom();

            //智能回覆用
            //// 延遲回覆 ms
            //setTimeout(() => {
            //    this.isTyping = false;
            //    this.messages.push({
            //        type: "received",
            //        content: this.getSmartReply(userMessage),
            //    });

            //    this.$nextTick(() => {
            //        this.scrollToBottom();
            //    });
            //}, 500);
        },

        scrollToBottom() {
            const container = this.$refs.messagesContainer;
            container.scrollTop = container.scrollHeight;
        },

        addMessage(messageData) {
            console.log("addMessage 開始執行");
            try {
                // 創建新的消息對象
                const newMessage = {
                    Type: messageData.type,
                    Content: messageData.content,
                    Time: messageData.time
                };
                console.log("newMessage 已創建:", newMessage);

                // 直接修改數組（Vue 3 不需要 Vue.set）
                this.currentChat.messages.push(newMessage);
                console.log("更新完成，當前 messages:", this.currentChat.messages);
                console.log("computed messages:", this.messages);

            } catch (error) {
                console.error("addMessage 執行錯誤:", error);
                console.error("錯誤堆疊:", error.stack);
                console.error("messageData:", messageData);
                console.error("currentChat 狀態:", this.currentChat);
            }
        }
    }
}).mount("#chatBlock");

