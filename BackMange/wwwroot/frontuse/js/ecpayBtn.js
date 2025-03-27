$("#payBtn").on('click', async (e) => {
    e.preventDefault();

    // 取得 taskId
    const taskId = document.getElementById("taskId").value;
    //const money = document.getElementById("caseBudget").value;
    const paymentData = {
        TotalAmount: document.getElementById("caseBudget").value,
        TradeDesc: "無",
        ItemName: document.getElementById("caseTitle").value,
        CustomField1: document.getElementById("caseTitle").value, //案件名稱
        CustomField2: taskId, //TaskId
        CustomField3: document.getElementById("posterId").value, //PosterId
        CustomField4: null, //WorkerId 由呼叫的API調取 TTask
        TaskId: taskId  // 加入 taskId
        //
    };
    console.log("paymentData:", paymentData );
    
    try {
        const response = await fetch('/api/Ecpay/CreatePayment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(paymentData)
        });

        if (response.ok) {
            const html = await response.text();
            const paymentWindow = window.open('', '_blank');
            paymentWindow.document.write(html);
            paymentWindow.document.close();
        } else {
            throw new Error('付款建立失敗');
        }

    } catch (err) {
        console.error('訂單建立失敗:', err);
        alert('訂單建立失敗，請稍後再試');
    }
});

