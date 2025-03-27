let dataTable;
$(document).ready(function () {
    loadDataTabel();
})

function loadDataTabel() {
    dataTable = $("#announceTable").DataTable({
        order: [5, 'desc'],
        "ajax": {
            url: '/Announce/GetAll',
            async: true,
            dataSrc: 'Data'
        },
        "columns": [
            { data: 'FAnnounceId', className: 'text-center', width: "5%" },
            { data: 'FTitle' },            
            { data: 'FCategoryName' },
            {
                data: 'PriorityLabel',
                render: function (data, type, row) {
                    switch (data) {
                        case '一般':
                            return '<p class="badge text-bg-primary" style="font-size:1em;margin:0px">一般</p>';
                        case '置頂':
                            return '<p class="badge text-bg-success" style="font-size:1em;margin:0px">置頂</p>';
                        case '緊急':
                            return '<p class="badge text-bg-danger" style="font-size:1em;margin:0px">緊急</p>';
                        default:
                            return '';
                    }
                }, width: "10%"
            },
            {
                data: 'Status',
                render: function (data, type, row) {
                    switch (data) {
                        case '草稿':
                            return '<p class="badge text-bg-secondary" style="font-size:1em;margin:0px">草稿</p>';
                        case '發布':
                            return '<p class="badge text-bg-success" style="font-size:1em;margin:0px">發布</p>';
                        case '下架':
                            return '<p class="badge text-bg-danger" style="font-size:1em;margin:0px">下架</p>';
                        default:
                            return '';
                    }
                }, width: "10%"
            },           
            {
                data: 'FUpdatedAt',
                render: function (data) {
                    // 格式化日期時間
                    if (data) {
                        const date = new Date(data);
                        const formattedDate = date.toLocaleString('zh-TW', {
                            year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour: '2-digit',
                            minute: '2-digit',
                            second: '2-digit',
                            hour12: false
                        });
                        return formattedDate; // 返回格式化后的日期时间
                    }
                    return ""; // 如果数据为空，返回空字符串
                }, width: "12%"
            },
            {
                data: 'FCreatedAt',
                render: function (data) {
                    // 格式化日期時間
                    if (data) {
                        const date = new Date(data);
                        const formattedDate = date.toLocaleString('zh-TW', {
                            year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour: '2-digit',
                            minute: '2-digit',
                            second: '2-digit',
                            hour12: false
                        });
                        return formattedDate; // 返回格式化后的日期时间
                    }
                    return ""; // 如果数据为空，返回空字符串
                }, width: "12%"
            },
            {
                data: 'FAnnounceId',
                render: function (data, type, row) {
                    let btnGroup = `<div class="btn-group">`;
                    if (row.Status === "發布") {
                        btnGroup += `<a href="/BAnnounce/Edit/${data}" class="btn btn-warning btn-sm">修改</a>
                        <a href="/BAnnounce/Removed/${data}" class="btn btn-danger btn-sm" onclick="return confirm('確定要下架嗎?')">下架</a>`;
                    } else {
                        btnGroup += `<a href="/BAnnounce/Edit/${data}" class="btn btn-warning btn-sm">修改</a>
    <a href="/BAnnounce/Republish/${data}" class="btn btn-success btn-sm" onclick="return confirm('要重新發布嗎?')">發布</a>`;
                    }

                    btnGroup += `</div>`;
                    return btnGroup;                  
                }, width:"8%" ,className: 'text-center' ,orderable: false // 禁用排序
            }
        ],
        //改成中文化
        language: {
            search: "搜尋：",
            lengthMenu: "顯示 _MENU_ 筆資料",
            info: "顯示第 _START_ 到 _END_ 筆資料，共 _TOTAL_ 筆",
            paginate: {
                first: "第一頁",
                last: "最後一頁",
                next: "下一頁",
                previous: "上一頁"
            },
            zeroRecords: "沒有找到符合的結果",
            emptyTable: "表格中沒有數據"
        }
    })
};

