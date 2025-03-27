let dataTable;

function loadDataTabel(categoryId) {
    if (dataTable) {
        dataTable.destroy(); // 銷毀現有 DataTable，避免重複初始化
    }
    dataTable = $("#FontannounceTable").DataTable({ 
        order: [[2, 'desc'], [3, 'desc']],
        "lengthChange": false,
        "info": false,
        "searching":false,
        "ajax": {
            url: '/Announce/GetAllFont',
            type: "GET",
            data: { categoryId: categoryId },
            async: true,
            dataSrc: "Data"
        },
        "columns": [           
            {
                data: 'FTitle', width:"75%",
                render: function (data, type, row) {
                    let result = `<a href="/FAnnounce/Details/${row.FAnnounceId}" class="btn btn-link" style="text-decoration:none;">`;
                    if (row.FPriority ==1) {
                        //一般                       
                        result += '<span class="badge text-bg-primary">一般</span>　';                       
                    }
                    else if (row.FPriority == 2) {
                        //緊急
                        result += '<span class="badge text-bg-danger">緊急</span>　';
                    }
                    else if (row.FPriority == 3) {
                        //置頂
                        result += '<span class="badge text-bg-success">置頂</span>　';
                    }
                    result += `${data}</a>`;
                    return result;
                }
            },
            {
                data: 'FCreatedAt', width: "25%",
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
                }
            },   
            {
                data: "FPriority", visible:false,
            },             
                   
        ],
        //改成中文化
        language: {
            search: "搜尋：", 
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

$(document).ready(function () {
    loadDataTabel(0);
})

$(document).ready(function () {
    $(".category-filter").click(function (e) {
        e.preventDefault(); // 阻止連結跳轉
        let categoryId = $(this).data("category-id"); // 取得 categoryId
        loadDataTabel(categoryId); // 重新加載 DataTables
    });
});

