// 通知發送函數
function sendNotification(options) {
    // 預設值設定
    const defaultOptions = {
        UserId: null,
        Content: "",
        Type: "System",  // 預設為系統通知
        RelatedId: null,
        SenderId: null,
        Icon: "bi-info-circle",  // 預設圖示
        Link: null
    };

    // 合併選項
    const notificationData = { ...defaultOptions, ...options };

    // 驗證必要欄位
    if (!notificationData.UserId) {
        console.error("必須提供接收者ID");
        return Promise.reject("必須提供接收者ID");
    }

    if (!notificationData.Content) {
        console.error("必須提供通知內容");
        return Promise.reject("必須提供通知內容");
    }

    // 發送通知
    return fetch('/api/Notify/SendNotification', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(notificationData)
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            console.log("通知發送成功");
            return data;
        } else {
            throw new Error(data.message || "發送失敗");
        }
    })
    .catch(error => {
        console.error("發送通知失敗:", error);
        throw error;
    });
}

// 批量發送通知函數
// function sendBulkNotifications(options) {
//     // 預設值設定
//     const defaultOptions = {
//         UserIds: [],
//         Content: "",
//         Type: "System",
//         RelatedId: null,
//         SenderId: null,
//         Icon: "bi-info-circle",
//         Link: null
//     };

//     // 合併選項
//     const notificationData = { ...defaultOptions, ...options };

//     // 驗證必要欄位
//     if (!notificationData.UserIds.length) {
//         console.error("必須提供接收者ID列表");
//         return Promise.reject("必須提供接收者ID列表");
//     }

//     if (!notificationData.Content) {
//         console.error("必須提供通知內容");
//         return Promise.reject("必須提供通知內容");
//     }

//     // 發送通知
//     return fetch('/api/Notify/SendBulkNotifications', {
//         method: 'POST',
//         headers: {
//             'Content-Type': 'application/json',
//         },
//         body: JSON.stringify(notificationData)
//     })
//     .then(response => response.json())
//     .then(data => {
//         if (data.success) {
//             console.log("批量通知發送成功");
//             return data;
//         } else {
//             throw new Error(data.message || "發送失敗");
//         }
//     })
//     .catch(error => {
//         console.error("批量發送通知失敗:", error);
//         throw error;
//     });
// }