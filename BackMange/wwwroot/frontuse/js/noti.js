//const { createApp, ref, onMounted, onUnmounted } = Vue;

createApp({
    setup() {
        const notifications = ref([]);
        const unreadCount = ref(0);
        const isDropdownVisible = ref(false);
        const showToast = ref(false);
        const toastMessage = ref('');
        const toastType = ref('info');
        const userId = parseInt(document.querySelector('#userId')?.value);

        // 載入通知
        const loadNotifications = async () => {
            try {
                
                if (isNaN(userId)) {
                    console.log("無效的用戶ID", "error");
                    //showToastMessage("無效的用戶ID", "error");
                    return;
                }

                const response = await fetch(`/api/Notify/LoadNotis/${userId}`);
                //console.log('Response status:', response.status);
                //console.log('Response statusText:', response.statusText);
                
                if (response.ok) {
                    const data = await response.json();
                    if (data.success) {
                        notifications.value = data.notifications.map(noti => ({
                            ...noti,
                            Type: noti.Type.toLowerCase(),
                            iconClass: getNotificationIconClass(noti.Type.toLowerCase()),
                            typeClass: getNotificationTypeClass(noti.Type.toLowerCase())
                        }));
                        //console.log("loadNotifications 資料:", notifications.value);
                        unreadCount.value = notifications.value.filter(n => !n.IsRead).length;
                    } else {
                        console.log('API 回傳失敗:', data.message);
                        //showToastMessage(data.message || "載入通知失敗", "error");
                    }
                } else {
                    const errorData = await response.text();
                    console.error('HTTP 錯誤狀態:', response.status);
                    console.error('錯誤詳情:', errorData);
                    //showToastMessage(`載入通知失敗 (${response.status})`, "error");
                }
            } catch (error) {
                console.error('請求發生錯誤:', error);
                //showToastMessage("網路連線錯誤", "error");
            }
        };
        
        

        // 根據通知類型取得對應的圖示類別
        const getNotificationIconClass = (type) => {
            const iconMap = {
                'system': 'bi-info-circle',
                'case': 'bi-briefcase',
                'chat': 'bi-chat-dots',
                'proposal': 'bi-file-earmark-text',
                'payment': 'bi-credit-card'
            };
            return iconMap[type] || 'bi-bell';
        };

        // 根據通知類型取得對應的 CSS 類別
        const getNotificationTypeClass = (type) => {
            return `${type.toLowerCase()}-notice`;
        };

        // 顯示/隱藏下拉選單
        const toggleDropdown = (event) => {
            if (!event.target.closest('.notification-dropdown')) {
                isDropdownVisible.value = !isDropdownVisible.value;
                event.stopPropagation();
            }
        };

        // 點擊外部關閉下拉選單
        const handleClickOutside = (event) => {
            if (!event.target.closest('.notification-icon')) {
                isDropdownVisible.value = false;
            }
        };

        // 標記全部已讀
        const markAllAsRead = async () => {
            try {
                // notifications.value.forEach(noti => noti.IsRead = true);
                // unreadCount.value = 0;

                const response = await fetch(`/api/Notify/MarkAllAsRead/${userId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                if (response.ok) {
                    const data = await response.json();
                    if (data.success) {
                        // 更新前端狀態
                        notifications.value.forEach(noti => noti.IsRead = true);
                        unreadCount.value = 0;
                        console.log('已將全部通知標記為已讀');
                        // showToastMessage("已將全部通知標記為已讀", "success");
                    } else {
                        console.error('標記已讀失敗:', data.message);
                        // showToastMessage(data.message || "標記已讀失敗", "error");
                    }
                }
                
            } catch (error) {
                //showToastMessage("標記已讀失敗", "error");
                console.error('標記已讀失敗:', error);
            }
        };

        const markAsRead = async (notificationId) => {
            try {
                const response = await fetch(`/api/Notify/MarkAsRead/${notificationId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
        
                if (response.ok) {
                    const data = await response.json();
                    if (data.success) {
                        console.log('通知已標記為已讀');
                        // 更新前端未讀數量
                        unreadCount.value--;
                    } else {
                        console.error('標記已讀失敗:', data.message);
                    }
                }
            } catch (error) {
                console.error('發生錯誤:', error);
            }
        }

        // 點擊個別通知
        const handleNotificationClick = async (notification) => {
            if (!notification.IsRead) {
                notification.IsRead = true;
                unreadCount.value = notifications.value.filter(n => !n.IsRead).length;
                // 這裡可以加入 API 呼叫來更新後端
                markAsRead(notification.NotificationId);
            }
            // if (notification.Link) {
            //     window.location.href = notification.Link;
            // }
        };

        // Toast 提示訊息
        const showToastMessage = (message, type = 'info') => {
            toastMessage.value = message;
            toastType.value = type;
            showToast.value = true;
            setTimeout(() => {
                showToast.value = false;
            }, 3000);
        };

        // 生命週期鉤子
        onMounted(() => {
            loadNotifications();
            document.addEventListener('click', handleClickOutside);
            setInterval(loadNotifications, 5000); //每5秒對後端調取該用戶之提醒
        });

        onUnmounted(() => {
            document.removeEventListener('click', handleClickOutside);
        });

        return {
            notifications,
            unreadCount,
            isDropdownVisible,
            showToast,
            toastMessage,
            toastType,
            toggleDropdown,
            markAllAsRead,
            handleNotificationClick,
            
        };
    },
    template: `
        <div @click="toggleDropdown">
            <i class="bi bi-bell"></i>
            <span class="notification-badge" v-show="unreadCount > 0">{{ unreadCount }}</span>
            <div class="notification-dropdown" :class="{ 'show': isDropdownVisible }">
                <div class="notification-header">
                    <h6>通知</h6>
                    <button class="mark-all-read" @click="markAllAsRead">全部標為已讀</button>
                </div>
                <div class="notification-list">
                    <a v-for="notification in notifications" 
                       :key="notification.NotificationId"
                       :href="notification.Link"
                       @click="handleNotificationClick(notification)"
                       :class="['notification-item', notification.typeClass, { 'unread': !notification.IsRead }]">
                        <div :class="['notification-icon-wrapper', notification.Type]">
                            <i :class="['bi', notification.iconClass]"></i>
                        </div>
                        <div class="notification-content">
                            <p class="notification-text">{{ notification.Content }}</p>
                            <span class="notification-time">{{ notification.TimeAgo }}</span>
                        </div>
                    </a>
                </div>
                <div class="notification-footer">
                    
                </div>
            </div>
        </div>
        <!-- Toast 組件 -->
        <div v-if="showToast" :class="['toast-notification', toastType]">
            <i :class="'bi bi-' + (toastType === 'success' ? 'check-circle' : 
                                  toastType === 'error' ? 'x-circle' : 
                                  toastType === 'warning' ? 'exclamation-triangle' : 'info-circle')"></i>
            <span>{{ toastMessage }}</span>
        </div>
    `
}).mount('.notification-icon');
