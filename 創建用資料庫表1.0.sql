create database GameCase
GO

USE GameCase
GO

---用戶類型表：儲存所有可能的用戶類型（工作者/發布者）
CREATE TABLE tUserTypes (
    FUserTypeID INT IDENTITY(1,1) PRIMARY KEY,    -- 用戶類型ID，自動遞增
    FUserTypeName NVARCHAR(20) NOT NULL UNIQUE    -- 用戶類型名稱，不可重複
);

-- 插入預設的用戶類型
INSERT INTO tUserTypes (FUserTypeName) VALUES 
('Worker'),   -- 工作者 (ID: 1)
('Poster');   -- 發布者 (ID: 2)

-- 用戶主表：儲存所有用戶的基本資料
CREATE TABLE tUsers (
    FUserID INT IDENTITY(1,1) PRIMARY KEY,        -- 用戶ID，自動遞增主鍵
    FFullName NVARCHAR(100) NOT NULL,             -- 用戶全名，必填
    FIdNumber NVARCHAR(10) NOT NULL,              -- 身分證字號（個人用戶），必填
    FCompanyNumber NVARCHAR(8),                   -- 統一編號（公司用戶）
    FEmail NVARCHAR(100) UNIQUE NOT NULL,         -- 電子郵件，唯一且必填
    FPasswordHash NVARCHAR(255),                  -- 密碼雜湊值（允許NULL，因為Google登入不需要密碼）
    FPhoneNumber NVARCHAR(20),                    -- 電話號碼
    FAddress NVARCHAR(255),                       -- 地址
    FBirthday DATE,                               -- 生日
    FGender NVARCHAR(10),                         -- 性別（M=男/F=女/O=其他）
    FProfileImageUrl NVARCHAR(255),               -- 大頭貼URL
    FLastLoginTime DATETIME,                      -- 最後登入時間
    FIsEmailVerified BIT DEFAULT 0,               -- 電子郵件是否驗證（0=未驗證/1=已驗證）
    FStatus TINYINT DEFAULT 1,                    -- 帳號狀態（1=啟用/2=停用/3=刪除）
    FSuspensionReason NVARCHAR(500),              -- 停用原因
    FSuspensionEndTime DATETIME,                  -- 停用結束時間
    FCreatedAt DATETIME DEFAULT GETDATE(),        -- 建立時間
    FUpdatedAt DATETIME,                          -- 更新時間
    FGoogleID NVARCHAR(100),                      -- Google ID（用於Google登入）
    FLoginType NVARCHAR(20) NOT NULL DEFAULT 'Local',  -- 登入類型（Local=一般登入/Google=Google登入）

    -- 性別欄位檢查：限制只能是 M/F/O
    CONSTRAINT CHK_Gender CHECK (FGender IN ('M', 'F', 'O')),

    -- 統一編號檢查：必須是8碼
    CONSTRAINT CHK_CompanyNumber CHECK (FCompanyNumber IS NULL OR LEN(FCompanyNumber) = 8),

    -- 登入類型檢查
    CONSTRAINT CHK_LoginType CHECK (FLoginType IN ('Local', 'Google'))
);


-- 建立 Google ID 的唯一索引（只對非NULL值）
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_GoogleID 
ON tUsers(FGoogleID)
WHERE FGoogleID IS NOT NULL;

-- 用戶類型關聯表：記錄用戶屬於哪種類型
CREATE TABLE tUserUserTypes (
    FUserID INT NOT NULL,                         -- 用戶ID
    FUserTypeID INT NOT NULL,                     -- 用戶類型ID
    PRIMARY KEY (FUserID, FUserTypeID),           -- 複合主鍵
    FOREIGN KEY (FUserID) REFERENCES tUsers(FUserID),          -- 關聯到用戶表
    FOREIGN KEY (FUserTypeID) REFERENCES tUserTypes(FUserTypeID) -- 關聯到用戶類型表
);

-- 插入測試用戶資料
INSERT INTO tUsers (
    FfullName, Femail, FidNumber, FpasswordHash, FphoneNumber, 
    Faddress, Fbirthday, Fgender, FprofileImageUrl, FlastLoginTime,
    FisEmailVerified, Fstatus, FsuspensionReason, FSuspensionEndTime,
    FcompanyNumber, FloginType, FgoogleId, FcreatedAt, FupdatedAt
) VALUES
-- 1. 一般用戶 密碼：Aa123123 (正常)
(N'張小明', 's1237798s@gmail.com', 'A123456789', 
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0912345678', N'台北市信義區信義路五段', '1990-05-15', 'M',
'profile1.jpg', '2024-03-15 10:30:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-01-01', '2024-03-15'),

-- 2. 一般用戶 密碼：Aa123123 (正常)
(N'李美玲', 'meiling@example.com', 'B234567890',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0923456789', N'台中市西區民生路', '1988-12-20', 'F',
'profile2.jpg', '2024-03-14 15:45:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-02-01', '2024-03-14'),

-- 3. 一般用戶 密碼：Aa123123 (正常)
(N'王大寶', 'dabao@example.com', 'C345678901',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0934567890', N'高雄市前金區中正路', '1995-08-25', 'M',
'profile3.jpg', '2024-03-13 09:15:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-01-15', '2024-03-13'),

-- 4. 企業用戶 密碼：Aa123123 (正常)
(N'大眾科技有限公司', 'tech@company.com', 'D456789012',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'02-27654321', N'新北市板橋區中山路', NULL, 'O',
'company1.jpg', '2024-03-12 14:20:00', 1, 1, NULL, NULL,
'12345678', 'Local', NULL, '2024-01-15', '2024-03-12'),

-- 5. 一般用戶 密碼：Aa123123 (正常)
(N'陳小華', 'xiaohua@example.com', 'E567890123',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0945678901', N'台南市東區長榮路', '1992-03-10', 'F',
'profile5.jpg', '2024-03-11 11:30:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-03-10', '2024-03-11'),

-- 6. 一般用戶 Google登入 (正常)
(N'林大雄', 'daxiong@gmail.com', 'F678901234',
NULL,
'0956789012', N'台中市南區建國路', '1987-07-15', 'M',
'profile6.jpg', '2024-03-10 16:45:00', 1, 1, NULL, NULL,
NULL, 'Google', '106789012340001', '2024-01-20', '2024-03-10'),

-- 7. 企業用戶 Google登入 (正常)
(N'創新數位股份有限公司', 'digital@gmail.com', 'G789012345',
NULL,
'02-87654321', N'台北市內湖區瑞光路', NULL, 'O',
'company2.jpg', '2024-03-09 13:20:00', 1, 1, NULL, NULL,
'87654321', 'Google', '107890123450001', '2024-02-15', '2024-03-09'),

-- 8. 一般用戶 Google登入 (正常)
(N'黃小琳', 'xiaolin@gmail.com', 'H890123456',
NULL,
'0967890123', N'新竹市東區光復路', '1994-11-30', 'F',
'profile8.jpg', '2024-03-08 10:15:00', 1, 1, NULL, NULL,
NULL, 'Google', '108901234560001', '2024-01-25', '2024-03-08'),

-- 9. 一般用戶 Google登入 (正常)
(N'吳大維', 'dawei@gmail.com', 'I901234567',
NULL,
'0978901234', N'高雄市鳳山區中山路', '1991-09-20', 'M',
'profile9.jpg', '2024-03-07 15:30:00', 1, 1, NULL, NULL,
NULL, 'Google', '109012345670001', '2024-01-28', '2024-03-07'),

-- 10. 一般用戶 Google登入 (正常)
(N'趙小美', 'xiaomei@gmail.com', 'J012345678',
NULL,
'0989012345', N'台中市西屯區文心路', '1993-04-05', 'F',
'profile10.jpg', '2024-03-06 12:45:00', 1, 1, NULL, NULL,
NULL, 'Google', '101234567890001', '2024-01-15', '2024-03-06'),

-- 11. 一般用戶 密碼：Aa123123 (正常)
(N'楊小凡', 'xiaofan@example.com', 'K123456789',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0912123456', N'台北市大安區忠孝東路', '1995-06-15', 'F',
'profile11.jpg', '2024-03-05 10:00:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-02-01', '2024-03-05'),

-- 12. 企業用戶 密碼：Aa123123 (正常)
(N'宏達科技股份有限公司', 'hongda@company.com', 'L234567890',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'02-23456789', N'新北市三重區重陽路', NULL, 'O',
'company3.jpg', '2024-03-04 14:20:00', 1, 1, NULL, NULL,
'23456789', 'Local', NULL, '2024-02-01', '2024-03-04'),

-- 13. 一般用戶 密碼：Aa123123 (正常)
(N'周小婷', 'xiaoting@example.com', 'M345678901',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0923234567', N'台中市北區三民路', '1989-08-25', 'F',
'profile13.jpg', '2024-03-03 09:30:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-02-10', '2024-03-03'),

-- 14. 一般用戶 密碼：Aa123123 (正常)
(N'蕭大志', 'dazhi@example.com', 'N456789012',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0934345678', N'高雄市苓雅區五福路', '1992-11-30', 'M',
'profile14.jpg', '2024-03-02 16:00:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-02-01', '2024-03-02'),

-- 15. 一般用戶 密碼：Aa123123 (正常)
(N'Leo', 'Leo12345@gmail.com', 'O567890123',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'02-34567890', N'台北市松山區民生東路', NULL, 'O',
'profile15.jpg', '2024-03-01 11:45:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-02-05', '2024-03-01'),
	
-- 16. 一般用戶 密碼：Aa123123 (正常)
(N'許小萱', 'xiaoxuan@example.com', 'P678901234',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0945456789', N'台南市中西區民族路', '1994-03-20', 'F',
'profile16.jpg', '2024-02-29 13:30:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-03-01', '2024-02-29'),

-- 17. 一般用戶 密碼：Aa123123 (正常)
(N'張大維', 'dawei2@example.com', 'Q789012345',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0956567890', N'新竹市北區光復路', '1990-07-15', 'M',
'profile17.jpg', '2024-02-28 15:15:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-02-01', '2024-02-28'),

-- 18. 企業用戶 密碼：Aa123123 (正常)
(N'數位方案有限公司', 'solution@company.com', 'R890123456',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'02-45678901', N'台北市信義區松仁路', NULL, 'O',
'company5.jpg', '2024-02-27 10:20:00', 1, 1, NULL, NULL,
'45678901', 'Local', NULL, '2024-02-08', '2024-02-27'),

-- 19. 一般用戶 密碼：Aa123123 (正常)
(N'林小玲', 'xiaoling@example.com', 'S901234567',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0967678901', N'台中市西屯區河南路', '1993-12-10', 'F',
'profile19.jpg', '2024-02-26 14:45:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-02-01', '2024-02-26'),

-- 20. 一般用戶 密碼：Aa123123 (正常)
(N'王小明', 'xiaoming@example.com', 'T012345678',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0978789012', N'高雄市前鎮區中山路', '1991-05-25', 'M',
'profile20.jpg', '2024-02-25 11:15:00', 1, 1, NULL, NULL,
NULL, 'Local', NULL, '2024-02-01', '2024-02-25'),

-- 21. 一般用戶 密碼：Aa123123 (未啟用)
(N'陳大寶', 'dabao2@example.com', 'U123456789',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0989890123', N'台北市中山區南京東路', '1988-09-30', 'M',
'profile21.jpg', '2024-02-24 16:30:00', 0, 0, NULL, NULL,
NULL, 'Local', NULL, '2024-02-12', '2024-02-24'),

-- 22. 企業用戶 密碼：Aa123123 (未啟用)
(N'創新軟體股份有限公司', 'software@company.com', 'V234567890',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'02-56789012', N'新北市新店區北新路', NULL, 'O',
'company6.jpg', '2024-02-23 13:45:00', 0, 0, NULL, NULL,
'56789012', 'Local', NULL, '2024-02-07', '2024-02-23'),

-- 23. 一般用戶 密碼：Aa123123 (未啟用)
(N'李小芳', 'xiaofang@example.com', 'W345678901',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0912345678', N'台中市南區建國北路', '1996-02-15', 'F',
'profile23.jpg', '2024-02-22 10:20:00', 0, 0, NULL, NULL,
NULL, 'Local', NULL, '2024-02-11', '2024-02-22'),

-- 24. 一般用戶 密碼：Aa123123 (未啟用)
(N'黃大明', 'daming@example.com', 'X456789012',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0923456789', N'台南市安平區建平路', '1987-11-20', 'M',
'profile24.jpg', '2024-02-21 15:30:00', 0, 0, NULL, NULL,
NULL, 'Local', NULL, '2024-02-01', '2024-02-21'),

-- 25. 企業用戶 密碼：Aa123123 (未啟用)
(N'雲端科技有限公司', 'cloud@company.com', 'Y567890123',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'02-67890123', N'台北市內湖區港墘路', NULL, 'O',
'company7.jpg', '2024-02-20 12:15:00', 0, 0, NULL, NULL,
'67890123', 'Local', NULL, '2024-02-06', '2024-02-20'),

-- 26. 一般用戶 密碼：Aa123123 (停用 - 違反社群守則 7天)
(N'周小明', 'xiaoming2@example.com', 'Z678901234',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0934567890', N'高雄市三民區九如路', '1995-07-25', 'M',
'profile26.jpg', '2024-02-19 09:45:00', 1, 2, N'違反社群守則', DATEADD(DAY, 7, GETDATE()),
NULL, 'Local', NULL, '2024-02-19', '2024-02-19'),

-- 27. 一般用戶 密碼：Aa123123 (停用 - 不當內容 3天)
(N'林小美', 'xiaomei2@example.com', 'AA89012345',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0945678901', N'新竹市東區光復路', '1992-04-10', 'F',
'profile27.jpg', '2024-02-18 14:30:00', 1, 2, N'發布不當內容', DATEADD(DAY, 3, GETDATE()),
NULL, 'Local', NULL, '2024-02-01', '2024-02-18'),

-- 28. 企業用戶 密碼：Aa123123 (停用 - 安全疑慮 14天)
(N'智慧物聯有限公司', 'iot@company.com', 'BB90123456',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'02-78901234', N'台北市南港區三重路', NULL, 'O',
'company8.jpg', '2024-02-17 11:30:00', 1, 2, N'帳號安全疑慮', DATEADD(DAY, 14, GETDATE()),
'78901234', 'Local', NULL, '2024-02-09', '2024-02-17'),

-- 29. 一般用戶 密碼：Aa123123 (停用 - 多次違規 30天)
(N'張小華', 'xiaohua2@example.com', 'CC01234567',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0956789012', N'台中市北屯區文心路', '1994-10-05', 'F',
'profile29.jpg', '2024-02-16 16:15:00', 1, 2, N'多次違規', DATEADD(DAY, 30, GETDATE()),
NULL, 'Local', NULL, '2024-02-01', '2024-02-16'),

-- 30. 一般用戶 密碼：Aa123123 (停用 - 騷擾行為 15天)
(N'劉大華', 'dahua@example.com', 'DD12345678',
'4yAybLMwm5ZPAXsRYPKgcvXVwpN8ZiHmMVwubqlF0cA=',
'0967890123', N'高雄市左營區博愛路', '1990-01-15', 'M',
'profile30.jpg', '2024-02-15 13:00:00', 1, 2, N'騷擾行為', DATEADD(DAY, 15, GETDATE()),
NULL, 'Local', NULL, '2024-02-01', '2024-02-15');

CREATE TABLE tVerifications (
    FVerificationID INT IDENTITY(1,1) PRIMARY KEY,   -- 驗證記錄 ID（自動遞增）
    FUserID INT NOT NULL,                            -- 關聯的用戶 ID（對應 tUsers.FUserID）
    FToken NVARCHAR(255) UNIQUE NOT NULL,           -- 驗證 Token（唯一，用於驗證）
    FTokenType NVARCHAR(20) NOT NULL,               -- Token 類型（EmailVerification / PasswordReset）
    FTokenSentTime DATETIME DEFAULT GETDATE(),      -- Token 發送時間
    FExpirationTime DATETIME NOT NULL,              -- Token 過期時間（通常 24 小時）
    FIsUsed BIT DEFAULT 0,                          -- 是否已使用（0=未使用，1=已使用）
    FUsedTime DATETIME,                             -- Token 使用時間（若已使用則記錄）

    -- 外鍵約束
    CONSTRAINT FK_Verifications_Users 
        FOREIGN KEY (FUserID) REFERENCES tUsers(FUserID) 
        ON DELETE CASCADE
);


-- 插入驗證記錄
INSERT INTO tVerifications (
    FUserID, FToken, FTokenType, FTokenSentTime, 
    FExpirationTime, FIsUsed, FUsedTime
) VALUES
-- 正常用戶的已使用 Email 驗證記錄（1-5, 11-20號）
(1, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-01-01 09:00:00',
'2024-01-02 09:00:00', 1, '2024-01-01 09:30:00'),

(2, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-01 10:00:00',
'2024-02-02 10:00:00', 1, '2024-02-01 10:15:00'),

(3, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-01-15 11:00:00',
'2024-01-16 11:00:00', 1, '2024-01-15 11:20:00'),

(4, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-01-15 13:00:00',
'2024-01-16 13:00:00', 1, '2024-01-15 13:10:00'),

(5, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-03-10 14:00:00',
'2024-03-11 14:00:00', 1, '2024-03-10 14:05:00'),

(11, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-01 09:00:00',
'2024-02-02 09:00:00', 1, '2024-02-01 09:10:00'),

(12, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-01 10:00:00',
'2024-02-02 10:00:00', 1, '2024-02-01 10:05:00'),

(13, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-10 11:00:00',
'2024-02-11 11:00:00', 1, '2024-02-10 11:15:00'),

(14, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-01 13:00:00',
'2024-02-02 13:00:00', 1, '2024-02-01 13:20:00'),

(15, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-05 14:00:00',
'2024-02-06 14:00:00', 1, '2024-02-05 14:10:00'),

(16, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-03-01 09:00:00',
'2024-03-02 09:00:00', 1, '2024-03-01 09:15:00'),

(17, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-01 10:00:00',
'2024-02-02 10:00:00', 1, '2024-02-01 10:20:00'),

(18, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-08 11:00:00',
'2024-02-09 11:00:00', 1, '2024-02-08 11:05:00'),

(19, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-01 13:00:00',
'2024-02-02 13:00:00', 1, '2024-02-01 13:10:00'),

(20, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-01 14:00:00',
'2024-02-02 14:00:00', 1, '2024-02-01 14:15:00'),

-- 未啟用用戶的未使用 Email 驗證記錄（21-25號）
(21, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-12 09:00:00',
'2024-02-13 09:00:00', 0, NULL),

(22, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-07 10:00:00',
'2024-02-08 10:00:00', 0, NULL),

(23, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-11 11:00:00',
'2024-02-12 11:00:00', 0, NULL),

(24, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-01 13:00:00',
'2024-02-02 13:00:00', 0, NULL),

(25, CONVERT(NVARCHAR(255), NEWID()), 'EmailVerification', '2024-02-06 14:00:00',
'2024-02-07 14:00:00', 0, NULL),

-- 一些用戶的密碼重設記錄（已使用）
(1, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-02-15 09:00:00',
'2024-02-16 09:00:00', 1, '2024-02-15 09:30:00'),

(3, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-02-16 10:00:00',
'2024-02-17 10:00:00', 1, '2024-02-16 10:15:00'),

(11, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-02-17 11:00:00',
'2024-02-18 11:00:00', 1, '2024-02-17 11:20:00'),

(13, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-02-18 13:00:00',
'2024-02-19 13:00:00', 1, '2024-02-18 13:10:00'),

(17, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-02-19 14:00:00',
'2024-02-20 14:00:00', 1, '2024-02-19 14:05:00'),

-- 一些用戶的密碼重設記錄（未使用）
(2, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-03-14 09:00:00',
'2024-03-15 09:00:00', 0, NULL),

(4, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-03-13 10:00:00',
'2024-03-14 10:00:00', 0, NULL),

(12, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-03-12 11:00:00',
'2024-03-13 11:00:00', 0, NULL),

(14, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-03-11 13:00:00',
'2024-03-12 13:00:00', 0, NULL),

(19, CONVERT(NVARCHAR(255), NEWID()), 'PasswordReset', '2024-03-10 14:00:00',
'2024-03-11 14:00:00', 0, NULL);


-- 接案者資料表：儲存接案者特定資訊   --2025/1/23更新 tWorkers資料表
CREATE TABLE tWorkers (
	FUserID INT PRIMARY KEY,                        -- 主鍵，同時綁定 tUsers 的 FUserID
	FCodeName NVARCHAR(50) NOT NULL,                -- 接案者暱稱
	FSkills NVARCHAR(255) NULL,                     -- 技能
	FExperienceYears INT NULL,                      -- 經驗年數
	FProfileDescription NVARCHAR(3000) NULL,        -- 簡短個人簡介
	FAboutMe NVARCHAR(MAX) NULL,                    -- 長篇自我介紹
	FWebsiteURL NVARCHAR(500) NULL,                 -- 相關連結（如個人網站、GitHub、IG 等）
	FServiceAvailability NVARCHAR(MAX) NULL,        -- 服務時段（由接案者自行填寫，建議格式附上範例）
	FCompletedTasksCount INT DEFAULT 0,             -- 已完成案件數
	FRating FLOAT CHECK (FRating BETWEEN 0 AND 5),  -- 評分（限制在 0~5）
	FIsVerified BIT DEFAULT 0,                      -- 是否已驗證
	FIsDeleted BIT DEFAULT 0,                       -- 是否啟用（或已刪除）
	FCreatedAt DATETIME DEFAULT GETDATE(),          -- 建立時間
	FUpdatedAt DATETIME NULL,                       -- 更新時間
	CONSTRAINT FK_Workers_Users FOREIGN KEY (FUserID)
		REFERENCES tUsers(FUserID)
);
INSERT INTO tWorkers 
(FUserID, FCodeName, FSkills, FExperienceYears, FProfileDescription, FWebsiteURL, FServiceAvailability, FCompletedTasksCount, FRating, FIsVerified, FIsDeleted)
VALUES 
(1, N'前端張小明', N'HTML,CSS,JavaScript,React,C#', 3,
N'我是一位專業的前端工程師，精通 HTML、CSS、JavaScript 及 React 框架，擅長開發高效、響應迅速且美觀的網站介面。我不僅專注於技術實現，更重視使用者體驗與創新設計，確保每個專案都能提供流暢且直覺的操作體驗。

在開發過程中，我熟悉模組化開發、效能優化、跨平台兼容性以及 SEO 最佳化，能夠透過最佳實踐提升網站速度與可維護性。此外，我具備良好的團隊協作能力，熟悉 Git 版本控制，能夠高效與設計師、後端工程師及產品經理合作，共同推動專案進展。

憑藉多年實戰經驗，我能夠快速解決技術問題，並持續學習最新的前端技術，如 Next.js、TypeScript 及 Web 性能優化策略，以打造兼具創新與實用性的網頁應用程式。',
N'https://www.google.com',
N'服務時段
星期一：07:00 - 24:00
星期二：07:00 - 24:00
星期三：07:00 - 24:00
星期四：07:00 - 24:00
星期五：07:00 - 24:00
星期六：09:00 - 21:00
星期日：09:00 - 21:00',
15, 4.8, 1, 0),

(2, N'設計師李美玲', N'UI設計,Figma,Photoshop,Illustrator,C#', 5,
N'我是一位資深的 UI/UX 設計師，擅長運用 Figma、Photoshop 及 Illustrator，創造兼具美感與實用性的設計作品。我專注於視覺呈現與使用者體驗，透過精心規劃的設計流程，確保每個作品都符合品牌調性與市場需求。

在設計過程中，我善於研究使用者行為與需求，運用 UX 研究方法，如使用者訪談、競品分析及 A/B 測試，來優化產品體驗。我也擁有豐富的設計系統與元件化設計經驗，能夠提高開發效率，確保設計一致性。

此外，我熟悉跨團隊合作模式，能夠與開發團隊無縫對接，提供高品質的設計規範與互動原型，確保最終產品符合預期。我持續關注最新設計趨勢，如微互動、無障礙設計 (A11Y) 及人工智慧驅動的設計，並將這些技術應用於實際專案，以打造更具創新與影響力的數位體驗。',
N'https://www.google.com',
N'服務時段
星期一：07:00 - 24:00
星期二：07:00 - 24:00
星期三：07:00 - 24:00
星期四：07:00 - 24:00
星期五：07:00 - 24:00
星期六：09:00 - 21:00
星期日：09:00 - 21:00',
25, 4.9, 1, 0),

(3, N'後端大神王大寶', N'C#,ASP.NET Core,SQL Server,Redis', 7,
N'自述：我專注於後端系統的設計與開發，精通C#、ASP.NET Core以及SQL Server，並運用Redis快取技術提升系統效能。多年來累積的架構設計與資料庫優化經驗，讓我能夠應對高流量環境下的各種挑戰，持續追求技術創新與穩定性。',
N'https://www.google.com',
N'服務時段
星期一：07:00 - 24:00
星期二：07:00 - 24:00
星期三：07:00 - 24:00
星期四：07:00 - 24:00
星期五：07:00 - 24:00
星期六：09:00 - 21:00
星期日：09:00 - 21:00',
30, 4.7, 1, 0),

(4, N'大眾科技有限公司', N'C#.NET、VS.NET、PHP、ASP、JAVA、AS3', 4,
N'自述：大眾科技承接各類網站與管理系統開發(C#.NET、VS.NET、PHP、ASP、JAVA、AS3等)、手機APP、企業管理系統及網站優化等軟體客製代工。共有二十多位程式人員、美術設計師、系統分析師與測試人員，承接許多國內外大型客戶如台北藝術大學、華文時尚雜誌、原住民委員會、金融研訓院、水利會、高雄市政府、台塑集團、空中大學、彪琥鞋業、日盛集團、寶成集團、伊綺美生物科技等企業政府專案。',
N'https://www.google.com',
N'服務時段
星期一：07:00 - 24:00
星期二：07:00 - 24:00
星期三：07:00 - 24:00
星期四：07:00 - 24:00
星期五：07:00 - 24:00
星期六：09:00 - 21:00
星期日：09:00 - 21:00',
20, 4.6, 1, 0),

(5, N'文案高手陳小華', N'文案撰寫,社群經營,內容行銷', 3,
N'自述：我專精於撰寫具有吸引力的品牌故事與行銷文案，擁有敏銳的文字表達能力和豐富的創意思維。通過深入理解品牌精神，我能夠製作出能夠觸動人心的文字內容，幫助企業提升品牌形象並強化市場競爭力。',
N'https://www.google.com',
N'服務時段
星期一：07:00 - 24:00
星期二：07:00 - 24:00
星期三：07:00 - 24:00
星期四：07:00 - 24:00
星期五：07:00 - 24:00
星期六：09:00 - 21:00
星期日：09:00 - 21:00',
18, 4.5, 1, 0),

(6, N'數據分析師Joe', N'Python,SQL,機器學習,Power BI,HTML,JavaScript', 6,
N'我是專業的數據分析師，擅長運用 Python 及 SQL 進行數據挖掘與分析，並利用機器學習技術提供精準的商業洞察。我曾參與多項企業數據可視化專案，熟練使用 Power BI 進行即時報表製作。我的專長在於將大量原始數據轉化為可行性策略，幫助企業提高營運效率。此外，我也提供 AI 預測模型開發與 A/B 測試分析，確保最佳決策品質。',
N'https://www.example.com',
N'服務時段
星期一至五：09:00 - 18:00
星期六：10:00 - 16:00',
40, 4.8, 1, 0),

(7, N'創新數位', N'Node.js,Vue.js,MySQL,API開發,JavaScript', 5,
N'資深的電商開發公司，專注於建立高效的網路購物平台與後端 API 服務。我精通 Node.js 和 Vue.js，能夠打造快速響應的前後端架構，並熟練操作 MySQL 進行數據管理。多年來，我協助多家企業優化電商網站的使用者體驗，提升轉換率與營收表現。我能夠提供從網站開發、系統優化到支付整合的完整解決方案。',
N'https://www.example.com',
N'服務時段
星期一至五：10:00 - 19:00
星期日：休息',
35, 4.7, 1, 0),

(8, N'自由插畫家Tina', N'Procreate, Photoshop, 平面設計,內容行銷', 4,
N'作為自由插畫家，我擁有豐富的商業插畫、品牌設計和視覺藝術創作經驗。我擅長使用 Procreate 和 Photoshop，能夠設計各種風格的數位插畫，並且能夠將品牌概念轉化為引人入勝的視覺作品。我曾與出版社、企業及個人創作者合作，提供封面設計、角色設計及社群媒體內容。我的作品強調創意、細節與獨特性，能有效提升品牌識別度。',
N'https://www.example.com',
N'服務時段
星期一至五：08:00 - 17:00
星期六：10:00 - 15:00',
28, 4.6, 1, 0),

(9, N'行銷顧問Kevin', N'數位行銷,SEO,廣告投放,品牌策略,內容行銷', 7,
N'身為資深行銷顧問，我專注於數位行銷策略、SEO 優化及廣告投放。我曾為多家企業規劃全方位行銷方案，涵蓋社群行銷、搜尋引擎排名提升及內容行銷。我擅長 Google Ads 及 Facebook 廣告，能精準鎖定受眾，提升品牌曝光度與轉換率。我的策略建立在數據分析基礎上，確保每一次行銷活動都能帶來最佳成效。',
N'https://www.example.com',
N'服務時段
星期一至五：09:30 - 18:30
星期六：09:00 - 14:00',
50, 4.9, 1, 0),

(10, N'系統架構師Ray', N'C#, ASP.NET, Kubernetes, 雲端架構', 10,
N'我是一位專業的系統架構師，專注於微服務架構與雲端部署。我擁有 10 年的開發與系統設計經驗，擅長使用 C# 和 ASP.NET 架構高效能應用。我熟練 Kubernetes 容器技術，並具備 AWS 雲端環境的最佳實踐經驗。我能夠為企業設計可擴展的軟體解決方案，確保系統的穩定性、安全性及效能。',
N'https://www.example.com',
N'服務時段
星期一至五：10:00 - 20:00
星期六：10:00 - 16:00',
55, 4.9, 1, 0),

(11, N'影片剪輯師Andy', N'Final Cut Pro, Premiere, After Effects', 5,
N'我是一名專業的影片剪輯師，精通 Final Cut Pro、Premiere 及 After Effects，擅長製作廣告影片、社群短片與 YouTube 內容。我曾與多家品牌合作，提供影片剪輯與後製服務，確保每支影片的視覺效果與故事敘述都能達到最佳水準。我熟悉影片色彩調整、動畫特效及音效混音，致力於提升視覺表現。',
N'https://www.example.com',
N'服務時段
星期一至五：10:00 - 19:00
星期六：09:00 - 14:00',
33, 4.7, 1, 0),

(12, N'App開發專家Tony', N'Flutter, Swift, Kotlin, Firebase,C#', 6,
N'我專注於開發跨平台 App，擁有 6 年的行動應用開發經驗。我擅長使用 Flutter、Swift 和 Kotlin，並熟悉 Firebase 雲端架構，提供即時資料庫與通知服務。我曾負責多款熱門應用程式的開發，涵蓋電商、社交媒體與金融科技領域。我重視效能與 UI/UX 設計，確保每款 App 具備流暢的使用體驗。',
N'https://www.example.com',
N'服務時段
星期一至五：10:00 - 20:00
星期六：11:00 - 17:00',
37, 4.8, 1, 0),

(13, N'全端開發者Tiny', N'Vue.js, Laravel, AWS, Docker,HTML', 8,
N'作為全端開發者，我精通 Vue.js 與 Laravel，並熟悉 AWS 雲端架構與 Docker 容器技術。我能夠開發高效能的 Web 應用，從前端 UI 設計到後端 API 整合皆具備完整的開發能力。我曾參與大型平台建置，確保系統的可擴展性與穩定性。我致力於技術創新，追求最佳的開發效率與效能優化。',
N'https://www.example.com',
N'服務時段
星期一至五：09:00 - 18:00
星期日：休息',
45, 4.9, 1, 0),

(14, N'UI/UX設計師蕭大志', N'Figma, Sketch, Adobe XD, UX Research,C#', 5,
N'我是 UI/UX 設計師，擅長使用 Figma、Sketch 及 Adobe XD 進行使用者介面設計與研究。我擁有豐富的產品設計經驗，致力於提升用戶體驗，讓產品更加直覺與易用。我曾與多家新創企業合作，提供視覺設計、使用者測試與產品策略顧問服務。',
N'https://www.example.com',
N'服務時段
星期一至五：10:00 - 19:00
星期六：09:00 - 13:00',
29, 4.7, 1, 0),
(15, N'網頁優化專家Leo', N'JavaScript, CSS, SEO, Lighthouse 分析,HTML', 7,
N'專注於網站速度優化與 SEO，擅長 JavaScript 與 CSS，運用 Lighthouse 分析工具提升網站表現。', 
N'https://www.example.com',
N'服務時段
星期一至五：09:30 - 18:30
星期六：10:00 - 14:00',
40, 4.8, 1, 0);



CREATE TABLE tPortfolios (
    PortfolioID INT IDENTITY(1,1) PRIMARY KEY,  -- 作品唯一識別碼，自動遞增
    FUserID INT NOT NULL,                        -- 對應接案者的 ID（外鍵，參照 tWorkers 的 FUserID）
    Title NVARCHAR(100) NOT NULL,                 -- 作品標題
    Description NVARCHAR(MAX) NULL,              -- 作品描述（若需要儲存較長的文字，可使用 NVARCHAR(MAX)）
    MediaURL NVARCHAR(255) NULL,                  -- 媒體檔案或作品連結（如圖片、影片、外部網站）
    CreatedAt DATETIME DEFAULT GETDATE(),        -- 作品上傳時間（預設為建立時的時間）
    UpdatedAt DATETIME NULL,                     -- 作品更新時間（可為 NULL，代表尚未更新）
    CONSTRAINT FK_Portfolios_Workers FOREIGN KEY (FUserID)
        REFERENCES tWorkers(FUserID)
);

CREATE TABLE tImages (
    FImageID INT IDENTITY(1,1) PRIMARY KEY,  -- 自增主鍵
    FUserID INT NOT NULL,                    -- 關聯到 tUsers
    FRole NVARCHAR(20) NOT NULL,             -- 使用者角色 (poster/worker)
    FCategory NVARCHAR(50) NOT NULL,         -- 圖片分類 (profile/portfolio/certificate)
    FImageName NVARCHAR(255) NOT NULL,       -- 檔案名稱，如 name.jpg
    FImagePath NVARCHAR(255) NOT NULL,       -- 相對路徑，如 uploads/users/1/worker/profile/
    FIsMain BIT DEFAULT 0,                   -- 是否為主要圖片
    FCreatedAt DATETIME DEFAULT GETDATE(),   -- 上傳時間
    CONSTRAINT FK_tImages_tUsers FOREIGN KEY (FUserID) REFERENCES tUsers(FUserID)
);

INSERT INTO tImages (FUserID, FRole, FCategory, FImageName, FImagePath, FIsMain, FCreatedAt)
VALUES 
(1, 'worker', 'profile', '2.jpg', 'uploads/users/1/worker/profile/2.jpg', 1, GETDATE()),
(1, 'worker', 'background', '14.jpg', 'uploads/users/1/worker/background/14.jpg', 1, GETDATE()),
(1, 'worker', 'background', '19.jpg', 'uploads/users/1/worker/background/19.jpg', 1, GETDATE()),
(1, 'worker', 'background', '9.jpg', 'uploads/users/1/worker/background/9.jpg', 1, GETDATE()),
(2, 'worker', 'background', '5.jpg', 'uploads/users/2/worker/background/5.jpg', 1, GETDATE()),
(2, 'worker', 'profile', '20.jpg', 'uploads/users/2/worker/profile/20.jpg', 1, GETDATE()),
(3, 'worker', 'background', '21.jpg', 'uploads/users/3/worker/background/21.jpg', 1, GETDATE()),
(3, 'worker', 'profile', '3.jpg', 'uploads/users/3/worker/profile/3.jpg', 1, GETDATE()),
(4, 'worker', 'background', '12.jpg', 'uploads/users/4/worker/background/12.jpg', 1, GETDATE()),
(4, 'worker', 'profile', '15.jpg', 'uploads/users/4/worker/profile/15.jpg', 1, GETDATE()),
(5, 'worker', 'background', '13.jpg', 'uploads/users/5/worker/background/13.jpg', 1, GETDATE()),
(5, 'worker', 'profile', '4.jpg', 'uploads/users/5/worker/profile/4.jpg', 1, GETDATE()),
(6, 'worker', 'background', '60.jpg', 'uploads/users/6/worker/background/60.jpg', 1, GETDATE()),
(6, 'worker', 'profile', '61.jpg', 'uploads/users/6/worker/profile/61.jpg', 1, GETDATE()),

(7, 'worker', 'background', '62.jpg', 'uploads/users/7/worker/background/62.jpg', 1, GETDATE()),
(7, 'worker', 'profile', '63.jpg', 'uploads/users/7/worker/profile/63.jpg', 1, GETDATE()),

(8, 'worker', 'background', '64.jpg', 'uploads/users/8/worker/background/64.jpg', 1, GETDATE()),
(8, 'worker', 'profile', '65.jpg', 'uploads/users/8/worker/profile/65.jpg', 1, GETDATE()),

(9, 'worker', 'background', '66.jpg', 'uploads/users/9/worker/background/66.jpg', 1, GETDATE()),
(9, 'worker', 'profile', '67.jpg', 'uploads/users/9/worker/profile/67.jpg', 1, GETDATE()),

(10, 'worker', 'background', '68.jpg', 'uploads/users/10/worker/background/68.jpg', 1, GETDATE()),
(10, 'worker', 'profile', '69.jpg', 'uploads/users/10/worker/profile/69.jpg', 1, GETDATE()),

(11, 'worker', 'background', '70.jpg', 'uploads/users/11/worker/background/70.jpg', 1, GETDATE()),
(11, 'worker', 'profile', '71.jpg', 'uploads/users/11/worker/profile/71.jpg', 1, GETDATE()),

(12, 'worker', 'background', '72.jpg', 'uploads/users/12/worker/background/72.jpg', 1, GETDATE()),
(12, 'worker', 'profile', '73.jpg', 'uploads/users/12/worker/profile/73.jpg', 1, GETDATE()),

(13, 'worker', 'background', '74.jpg', 'uploads/users/13/worker/background/74.jpg', 1, GETDATE()),
(13, 'worker', 'profile', '75.jpg', 'uploads/users/13/worker/profile/75.jpg', 1, GETDATE()),

(14, 'worker', 'background', '76.jpg', 'uploads/users/14/worker/background/76.jpg', 1, GETDATE()),
(14, 'worker', 'profile', '77.jpg', 'uploads/users/14/worker/profile/77.jpg', 1, GETDATE()),

(15, 'worker', 'background', '78.jpg', 'uploads/users/15/worker/background/78.jpg', 1, GETDATE()),
(15, 'worker', 'profile', '79.jpg', 'uploads/users/15/worker/profile/79.jpg', 1, GETDATE());


-- 創建發案者資料表
CREATE TABLE tPosters (
    FUserID INT PRIMARY KEY,                        -- 主鍵，直接使用用戶ID
    FCompanyName NVARCHAR(255),                    -- 公司名稱
    FCompanyRegistrationNumber NVARCHAR(50),       -- 公司註冊編號
    FReputationScore FLOAT CHECK (FReputationScore BETWEEN 0 AND 5), -- 信譽評分
    FIsVerified BIT DEFAULT 0,                     -- 是否已驗證
    FIsDeleted BIT DEFAULT 0,                      -- 是否已刪除
    FCreatedAt DATETIME DEFAULT GETDATE(),         -- 建立時間
    FUpdatedAt DATETIME,                           -- 更新時間
    CONSTRAINT FK_Posters_Users FOREIGN KEY (FUserID) REFERENCES tUsers(FUserID)
);

--新增發案者資料
INSERT INTO [dbo].[tPosters] 
([FUserID], [FCompanyName], [FCompanyRegistrationNumber], [FReputationScore], [FIsVerified], [FIsDeleted], [FCreatedAt], [FUpdatedAt]) 
VALUES
    (1, N'張小明', null, 1, 0, 0, GETDATE(), GETDATE()),
    (2, N'李美玲', null, 2, 0, 0, GETDATE(), GETDATE()),
    (3, N'王大寶',null, 3, 0, 0, GETDATE(), GETDATE()),
    (4, N'大眾科技有限公司', N'12345678', 4, 0, 0, GETDATE(), GETDATE()),
    (5, N'陳小華',null, 5, 0, 0, GETDATE(), GETDATE()),
    (6, N'林大雄', null, 3.5, 0, 0, GETDATE(), GETDATE()),
    (7, N'創新數位股份有限公司', N'87654321', 4.2, 0, 0, GETDATE(), GETDATE()),
    (8, N'黃小琳', null, 4.8, 0, 0, GETDATE(), GETDATE()),
    (9, N'吳大維', null, 3.9, 0, 0, GETDATE(), GETDATE()),
    (10, N'趙小美', null, 4.5, 0, 0, GETDATE(), GETDATE()),
    (11, N'楊小凡',null, 3.2, 0, 0, GETDATE(), GETDATE()),
    (12, N'宏達科技股份有限公司', N'23456789', 4.0, 0, 0, GETDATE(), GETDATE()),
    (13, N'周小婷', null, 3.7, 0, 0, GETDATE(), GETDATE()),
    (14, N'蕭大志', null, 4.6, 0, 0, GETDATE(), GETDATE()),
    (15, N'Leo',null, 3.8, 0, 0, GETDATE(), GETDATE());

-- 任務資料表：儲存所有案件資訊
CREATE TABLE tTasks (
    FTaskID INT IDENTITY(1,1) PRIMARY KEY,   -- 任務ID，自動遞增
    FPosterID INT NOT NULL,                  -- 發案人ID
    FCategoryID INT NOT NULL,                -- 任務類別ID
    FTitle NVARCHAR(255) NOT NULL,           -- 任務標題
    FDescription NVARCHAR(MAX) NOT NULL,     -- 任務內容
    FBudget INT CHECK (FBudget > 0) NOT NULL,-- 預算（必須大於 0）
    FLocation NVARCHAR(255) NOT NULL,        -- 工作地點
    FLocationDetail NVARCHAR(255) NOT NULL,  -- 工作詳細地址
    FMember NVARCHAR(50) NOT NULL,           -- 聯絡人姓名
    FPhone NVARCHAR(50) NOT NULL,            -- 聯絡電話
    FEmail NVARCHAR(50) NOT NULL,            -- 聯絡Email
    FStatus NVARCHAR(20) NOT NULL,           -- 任務狀態  (待審核、發佈中、進行中、已完成、已取消)
    FDeadline DATETIME NOT NULL,             -- 任務截止日期
    FCreatedAt DATETIME DEFAULT GETDATE(),   -- 建立時間（預設為當前時間）
    FUpdatedAt DATETIME DEFAULT GETDATE(),   -- 更新時間（預設為當前時間）
    Ftimage NVARCHAR(MAX) NULL               -- 圖片路徑（允許多張圖片）
);


--創建任務類別資料表
CREATE TABLE tCategory (
    ID INT IDENTITY(1,1) PRIMARY KEY,   -- 自動遞增主鍵
    JobName NVARCHAR(255) NOT NULL      -- 任務類別名稱
);

--新增任務類別資料
INSERT INTO tCategory (JobName) VALUES 
    (N'設計'),
    (N'程式開發'),
    (N'行銷'),
    (N'翻譯'),
    (N'教育訓練'),
    (N'文書處理'),
    (N'網頁設計'),
    (N'手機應用開發'),
    (N'資料分析'),
    (N'社群經營'),
    (N'影片剪輯'),
    (N'攝影'),
    (N'影像後製'),
    (N'聲音編輯'),
    (N'SEO 優化'),
    (N'UI/UX 設計'),
    (N'品牌設計'),
    (N'電子商務'),
    (N'數據輸入'),
    (N'客服支援'),
    (N'專案管理'),
    (N'財務管理'),
    (N'法律諮詢'),
    (N'遊戲開發'),
    (N'產品設計'),
    (N'機器學習'),
    (N'AI 模型訓練'),
    (N'3D 建模'),
    (N'插畫繪製'),
    (N'社群廣告投放');

--插入案件資訊
INSERT INTO tTasks (
    FPosterID, FCategoryID, FTitle, FDescription, FBudget, FLocation,
    FLocationDetail, FMember, FPhone, FEmail, FStatus, FDeadline, Ftimage
) VALUES
-- 張小明的任務 (10筆)
(1, 1, N'LOGO設計', N'設計現代企業LOGO', 5000, N'台北市', N'信義區文林路', N'張小明', N'0912345678', N's1237798s@gmail.com', N'發佈中', '2025-03-10 12:00:00', N'/uploads/task1.jpg'),
(1, 2, N'後端開發', N'Node.js API開發', 30000, N'台北市', N'內湖區', N'張小明', N'0912345678', N's1237798s@gmail.com', N'發佈中', '2025-03-15 18:00:00', N'/uploads/task2.jpg'),
(1, 3, N'社群行銷', N'規劃行銷策略', 15000, N'台北市', N'松山區', N'張小明', N'0912345678', N's1237798s@gmail.com', N'發佈中', '2025-03-20 09:30:00', N'/uploads/task3.jpg'),
(1, 4, N'技術文件翻譯', N'英文翻譯5000字', 8000, N'台北市', N'中山區', N'張小明', N'0912345678', N's1237798s@gmail.com', N'發佈中', '2025-03-25 14:00:00', N'/uploads/task4.jpg'),
(1, 5, N'Python AI 訓練', N'機器學習模型優化', 40000, N'台北市', N'士林區', N'張小明', N'0912345678', N's1237798s@gmail.com', N'進行中', '2025-03-30 16:45:00', N'/uploads/task5.jpg'),
(1, 6, N'文書整理', N'PDF 文件分類', 3000, N'台北市', N'北投區', N'張小明', N'0912345678', N's1237798s@gmail.com', N'進行中', '2025-04-01 10:00:00', N'/uploads/task6.jpg'),
(1, 7, N'網站開發', N'公司形象網站', 25000, N'台北市', N'信義區', N'張小明', N'0912345678', N's1237798s@gmail.com', N'已完成', '2025-04-10 12:30:00', N'/uploads/task7.jpg'),
(1, 8, N'App開發', N'Android & iOS 開發', 50000, N'台北市', N'中正區', N'張小明', N'0912345678', N's1237798s@gmail.com', N'已完成', '2025-04-15 15:00:00', N'/uploads/task8.jpg'),
(1, 9, N'數據分析', N'銷售數據分析', 12000, N'台北市', N'南港區', N'張小明', N'0912345678', N's1237798s@gmail.com', N'待審核', '2025-04-20 17:00:00', N'/uploads/task9.jpg'),
(1, 10, N'社群管理', N'Instagram & Facebook', 10000, N'台北市', N'大安區', N'張小明', N'0912345678', N's1237798s@gmail.com', N'待審核', '2025-04-25 19:30:00', N'/uploads/task10.jpg'),

-- 李美玲的任務 (10筆)
(2, 1, N'影片剪輯', N'企業影片剪輯', 20000, N'台中市', N'北區', N'李美玲', N'0923456789', N'meiling@example.com', N'發佈中', '2025-05-10 12:00:00', N'/uploads/task11.jpg'),
(2, 2, N'攝影拍攝', N'產品攝影', 15000, N'台中市', N'南屯區', N'李美玲', N'0923456789', N'meiling@example.com', N'發佈中', '2025-06-01 15:00:00', N'/uploads/task12.jpg'),
(2, 3, N'影像後製', N'影片特效 & 調色', 18000, N'台中市', N'西屯區', N'李美玲', N'0923456789', N'meiling@example.com', N'發佈中', '2025-06-10 17:30:00', N'/uploads/task13.jpg'),
(2, 4, N'聲音編輯', N'音頻剪輯與音效', 12000, N'台中市', N'東區', N'李美玲', N'0923456789', N'meiling@example.com', N'發佈中', '2025-06-15 11:00:00', N'/uploads/task14.jpg'),
(2, 5, N'SEO 優化', N'網站 SEO 提升', 14000, N'台中市', N'北屯區', N'李美玲', N'0923456789', N'meiling@example.com', N'進行中', '2025-06-20 09:00:00', N'/uploads/task15.jpg'),
(2, 6, N'海報設計', N'活動宣傳海報', 7000, N'台中市', N'南區', N'李美玲', N'0923456789', N'meiling@example.com', N'進行中', '2025-06-25 14:00:00', N'/uploads/task16.jpg'),
(2, 7, N'電子書製作', N'PDF & EPUB 設計', 10000, N'台中市', N'大里區', N'李美玲', N'0923456789', N'meiling@example.com', N'已完成', '2025-07-01 10:30:00', N'/uploads/task17.jpg'),
(2, 8, N'WordPress 建站', N'企業官網建置', 25000, N'台中市', N'霧峰區', N'李美玲', N'0923456789', N'meiling@example.com', N'已完成', '2025-07-05 18:00:00', N'/uploads/task18.jpg'),
(2, 9, N'新聞稿撰寫', N'品牌新聞稿', 5000, N'台中市', N'豐原區', N'李美玲', N'0923456789', N'meiling@example.com', N'發佈中', '2025-07-10 09:00:00', N'/uploads/task19.jpg'),
(2, 1, N'簡報設計', N'專業簡報設計', 8000, N'台中市', N'潭子區', N'李美玲', N'0923456789', N'meiling@example.com', N'待審核', '2025-07-15 16:00:00', N'/uploads/task20.jpg'),

-- 王大寶的任務 (10筆) [類似結構，省略]
(3, 1, N'企業網站開發', N'開發RWD響應式網站', 28000, N'高雄市', N'苓雅區五福路', N'王大寶', N'0934567890', N'dabao@example.com', N'發佈中', '2025-03-14 13:30:00', N'/uploads/task21.jpg'),
(3, 2, N'商品攝影', N'拍攝電商商品圖片並修圖', 15000, N'高雄市', N'左營區博愛路', N'王大寶', N'0934567890', N'dabao@example.com', N'發佈中', '2025-03-19 17:00:00', N'/uploads/task22.jpg'),
(3, 3, N'影片腳本撰寫', N'撰寫品牌宣傳影片腳本', 9000, N'高雄市', N'前鎮區凱旋路', N'王大寶', N'0934567890', N'dabao@example.com', N'進行中', '2025-03-24 15:00:00', N'/uploads/task23.jpg'),
(3, 4, N'行銷活動規劃', N'制定社群行銷活動策略', 13000, N'高雄市', N'鳳山區中山路', N'王大寶', N'0934567890', N'dabao@example.com', N'已完成', '2025-03-30 19:30:00', N'/uploads/task24.jpg'),
(3, 5, N'動畫製作', N'製作2D動畫短片', 35000, N'高雄市', N'鼓山區明華路', N'王大寶', N'0934567890', N'dabao@example.com', N'發佈中', '2025-04-05 16:00:00', N'/uploads/task25.jpg'),
(3, 6, N'3D建模與渲染', N'為產品製作高品質3D模型與渲染圖', 40000, N'高雄市', N'鹽埕區五福四路', N'王大寶', N'0934567890', N'dabao@example.com', N'進行中', '2025-04-12 12:00:00', N'/uploads/task26.jpg'),
(3, 7, N'虛擬主播形象設計', N'設計一個Vtuber角色並製作Live2D動畫', 28000, N'高雄市', N'楠梓區加昌路', N'王大寶', N'0934567890', N'dabao@example.com', N'發佈中', '2025-04-18 14:30:00', N'/uploads/task27.jpg'),
(3, 8, N'企業報表自動化', N'使用Python或Excel VBA自動產出報表', 18000, N'高雄市', N'三民區建國一路', N'王大寶', N'0934567890', N'dabao@example.com', N'發佈中', '2025-04-25 10:00:00', N'/uploads/task28.jpg'),
(3, 9, N'電子商務網站優化', N'優化電商網站效能，提升用戶體驗', 30000, N'高雄市', N'鳳山區光遠路', N'王大寶', N'0934567890', N'dabao@example.com', N'發佈中', '2025-05-02 18:00:00', N'/uploads/task29.jpg'),
(3, 20, N'短影音拍攝與剪輯', N'為社群平台製作創意短影音', 15000, N'高雄市', N'前鎮區中山二路', N'王大寶', N'0934567890', N'dabao@example.com', N'待審核', '2025-05-08 15:00:00', N'/uploads/task30.jpg');

-- 發案者資料表：儲存發案者特定資訊 1/28新增關注欄位 --做程式要注意不能追蹤自己
CREATE TABLE tWorkerFollows (
    FFollowID INT IDENTITY(1,1) PRIMARY KEY,
    FFollowerID INT NOT NULL,                 
    FWorkerUserID INT NOT NULL,              
    FCreatedAt DATETIME DEFAULT GETDATE(),    

    -- 追蹤者必須是用戶
    CONSTRAINT FK_WorkerFollows_Follower 
        FOREIGN KEY (FFollowerID) 
        REFERENCES tUsers(FUserID) 
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    -- 被追蹤者必須是工作者
    CONSTRAINT FK_WorkerFollows_Worker 
        FOREIGN KEY (FWorkerUserID) 
        REFERENCES tUsers(FUserID)  
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    -- 防止重複追蹤
    CONSTRAINT UQ_WorkerFollows 
        UNIQUE (FFollowerID, FWorkerUserID)
);
INSERT INTO tWorkerFollows (FFollowerID, FWorkerUserID, FCreatedAt) VALUES
(1, 2, '2024-03-01 10:00:00'),  -- 張小明追蹤設計師Amy
(1, 3, '2024-03-02 14:30:00'),  -- 張小明追蹤後端大神
(1, 5, '2024-03-03 16:45:00'),  -- 張小明追蹤文案高手
(2, 1, '2024-03-10 09:15:00'),  -- 王大寶追蹤前端小王
(2, 3, '2024-03-11 11:20:00'),  -- 王大寶追蹤設計師Amy
(2, 4, '2024-03-12 13:40:00'),
(1, 9, '2024-03-20 09:08:00'),
(1, 15, '2024-03-08 14:02:00'),
(1, 7, '2024-03-15 13:33:00'),
(1, 11, '2024-03-17 11:58:00'),
(1, 14, '2024-03-09 15:00:00'),
(3, 10, '2024-03-13 09:39:00'),
(3, 14, '2024-03-14 11:35:00'),
(4, 10, '2024-03-20 17:10:00'),
(5, 11, '2024-03-10 13:33:00'),
(6, 2, '2024-03-11 09:36:00'),
(7, 1, '2024-03-03 12:40:00'),
(8, 3, '2024-03-19 16:52:00'),
(8, 7, '2024-03-01 15:21:00');


-- 追蹤任務的資料表
CREATE TABLE tTaskFollows (
    FFollowID INT IDENTITY(1,1) PRIMARY KEY,
    FFollowerID INT NOT NULL,                 
    FTaskID INT NOT NULL,                     
    FCreatedAt DATETIME DEFAULT GETDATE(),    

    CONSTRAINT FK_TaskFollows_Follower 
        FOREIGN KEY (FFollowerID) 
        REFERENCES tUsers(FUserID) 
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT FK_TaskFollows_Task 
        FOREIGN KEY (FTaskID) 
        REFERENCES tTasks(FTaskID) 
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,

    CONSTRAINT UQ_TaskFollows 
        UNIQUE (FFollowerID, FTaskID)
);
INSERT INTO tTaskFollows (FFollowerID, FTaskID, FCreatedAt) VALUES
(1, 11, '2024-03-14 08:01:00'),
(1, 12, '2024-03-03 11:14:00'),
(1, 13, '2024-03-17 17:01:00'),
(1, 14, '2024-03-18 11:45:00'),
(2, 1, '2024-03-18 14:14:00'),
(2, 2, '2024-03-15 17:17:00'),
(3, 1, '2024-03-01 10:44:00'),
(3, 2, '2024-03-14 13:17:00');

CREATE TABLE tAdminStatus (
	FStatusID TINYINT PRIMARY KEY,           -- 狀態ID
	FStatusName NVARCHAR(20) NOT NULL,       -- 狀態名稱
	FDescription NVARCHAR(100),              -- 狀態描述
	FCreatedAt DATETIME DEFAULT GETDATE()    -- 建立時間
);

-- 員工資料表：儲存平台員工的基本資料
INSERT INTO tAdminStatus (FStatusID, FStatusName, FDescription) VALUES
(1, N'啟用中', N'管理員帳號正常使用中'),
(2, N'審核中', N'管理員帳號等待審核'),
(3, N'已刪除', N'管理員帳號已被刪除'); 

CREATE TABLE tAdmins (
    FAdminID INT IDENTITY(1,1) PRIMARY KEY,      -- 主鍵
    FAdminNo NVARCHAR(20) UNIQUE NOT NULL,       -- 管理員編號 (A001)
    FFullName NVARCHAR(100) NOT NULL,            -- 管理員全名
    FEmail NVARCHAR(100) UNIQUE NOT NULL,        -- 電子郵件
    FAdmPassword NVARCHAR(255) NOT NULL,        -- 密碼
    FMobilePhone NVARCHAR(20),                   -- 手機號碼
    FAdminLevel INT NOT NULL,                    -- 管理員等級 (1=一般, 2=進階, 3=超級管理員)
    FStatusID TINYINT NOT NULL DEFAULT 1,        -- 狀態ID
    FCreatedAt DATETIME DEFAULT GETDATE(),       -- 建立時間
    FUpdatedAt DATETIME,                         -- 更新時間
    CONSTRAINT FK_Admin_Status FOREIGN KEY (FStatusID) 
    REFERENCES tAdminStatus(FStatusID)
);

-- 然後設定用戶類型關聯
INSERT INTO tAdmins (
    FAdminNo, FFullName, FEmail, FAdmPassword, 
    FMobilePhone, FAdminLevel, 
    FStatusID, FCreatedAt, FUpdatedAt
) VALUES 
('A001', N'蔡大花', 'admin1@test.com', '1234', 
'0912345678', 1, 
1, GETDATE(), GETDATE()),

('A002', N'王小豪', 'admin2@test.com', '1234', 
'0987654321', 2, 
1, GETDATE(), GETDATE());

--公告類別表
CREATE TABLE tAnnounceCategories(
	fCategoryId INT PRIMARY KEY IDENTITY(1,1),
	fCategoryName NVARCHAR(50) NOT NULL UNIQUE,
);
 
--公告事項資料表
CREATE TABLE tAnnounce (
	fAnnounceId INT PRIMARY KEY IDENTITY(1,1),
	fTitle NVARCHAR(100) NOT NULL,
	fContent Text NOT NULL,
	fCategoryId INT FOREIGN KEY REFERENCES tAnnounceCategories(fCategoryId) NOT NULL,
	fPriority INT DEFAULT 1 NOT NULL, --(1:一般 , 2:緊急 , 3:置頂)
	fCreatedAt DATETIME DEFAULT GETDATE() NOT NULL,
	fUpdatedAt DATETIME DEFAULT GETDATE() NOT NULL,
	Status NVARCHAR(10)  NOT NULL CHECK (status IN ('草稿', '發布', '下架')),
);

--插入公告種類
INSERT INTO tAnnounceCategories (fCategoryName)
VALUES 
('系統公告'),
('功能更新'),
('安全提醒'),
('最新消息'),
('活動與優惠');

--插入公告事項
INSERT INTO tAnnounce (fTitle, fContent, fCategoryId, fPriority, Status)
VALUES 
-- 系統公告
('系統維護通知', '系統將於今晚進行維護，期間服務暫停，請見諒。', 1, 3, '發布'),
('版本更新提示', '系統已更新至最新版本，請重新登入以完成配置。', 1, 2, '發布'),
('伺服器升級計畫', '我們將於下周進行伺服器升級，可能會影響部分功能。', 1, 3, '草稿'),

-- 功能更新
('新功能介紹 - 即時聊天', '我們新增了即時聊天功能，讓您與案主溝通更加方便。', 2, 2, '發布'),
('報表功能上線', '用戶現在可以在個人中心查看詳細的接案報表。', 2, 1, '發布'),
('智能推薦系統', '系統優化了案件推薦功能，匹配更準確！', 2, 2, '發布'),

-- 安全提醒
('防詐騙警告', '近期出現假冒案主的行為，請勿提供私人資訊。', 3, 3, '發布'),
('帳號安全建議', '請定期更換密碼，啟用雙重驗證以保障帳號安全。', 3, 2, '發布'),
('可疑活動提醒', '系統檢測到您的帳號有異常登入，請立即更改密碼。', 3, 3, '下架'),

-- 最新消息
('全新用戶界面', '平台更新了全新的用戶界面，使用體驗更加流暢。', 4, 2, '發布'),
('用戶數突破10萬', '感謝所有用戶的支持，我們的用戶數已突破10萬！', 4, 1, '發布'),
('合作夥伴新增', '我們與多家知名企業達成合作，提供更多機會！', 4, 2, '草稿');

--　===== 聊天紀錄 =====
-- 聊天室創建紀錄
CREATE TABLE tChats (
    fChatId INT PRIMARY KEY IDENTITY(1,1), 
    fPosterId INT NOT NULL,
    fWorkerId INT NOT NULL,
    fCreatedAt DATETIME DEFAULT GETDATE(),

	-- 刪除用戶時自動刪除相關聊天
    --FOREIGN KEY (BuyerId) REFERENCES Users(UserId) ON DELETE CASCADE, 
    --FOREIGN KEY (SellerId) REFERENCES Users(UserId) ON DELETE CASCADE
	FOREIGN KEY (fWorkerId) REFERENCES tUsers(FUserID) , 
    FOREIGN KEY (fPosterId) REFERENCES tUsers(FUserID) ,
	
);

-- 訊息紀錄
CREATE TABLE tMessagesHistory (
    fMessageId INT PRIMARY KEY IDENTITY(1,1), -- 自動遞增的主鍵
    fChatId INT NOT NULL ,
    fSenderId INT NOT NULL, -- 發送者的 UserId
    fMessageText NVARCHAR(MAX) NOT NULL, -- 使用 NVARCHAR(MAX) 以支援長訊息
    fSentAt DATETIME NOT NULL DEFAULT GETDATE(),
    --FOREIGN KEY (ChatId) REFERENCES Chats(ChatId) ON DELETE CASCADE, 
    --FOREIGN KEY (SenderId) REFERENCES Users(UserId) ON DELETE CASCADE
	fIsRead BIT NOT NULL DEFAULT 0,
	FOREIGN KEY (fChatId) REFERENCES tChats(fChatId), 
    FOREIGN KEY (fSenderId) REFERENCES tUsers(FUserID)
);

--插入訊息紀錄
/* SET IDENTITY_INSERT [dbo].[tMessagesHistory] ON 
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (1, 1, 1, N'您好，我對A案有興趣', CAST(N'2025-02-24T16:11:48.677' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (2, 1, 3, N'好的請問對於A案的案件內容是否有疑問呢?', CAST(N'2025-02-24T16:12:51.073' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (3, 1, 1, N'金額太低', CAST(N'2025-02-24T16:13:59.827' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (4, 1, 3, N'抱歉目前公司資金不足', CAST(N'2025-02-24T16:14:17.953' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (5, 1, 1, N'測試', CAST(N'2025-02-24T18:35:32.773' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (6, 3, 3, N'測試', CAST(N'2025-02-24T18:39:34.727' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (7, 3, 3, N'我想接案!!!!', CAST(N'2025-02-24T18:40:01.713' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (8, 3, 3, N'讓我接拜託!', CAST(N'2025-02-24T18:40:06.063' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (9, 3, 2, N'請先上傳您的簡歷', CAST(N'2025-02-24T18:45:51.043' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (10, 1, 1, N'收到測試', CAST(N'2025-02-24T19:21:04.493' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (11, 1, 3, N'是我收到測試', CAST(N'2025-02-24T19:22:56.627' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (12, 10, 3, N'黃小姐您好 我要接案', CAST(N'2025-02-24T19:41:34.990' AS DateTime), 0)
GO
INSERT [dbo].[tMessagesHistory] ([fMessageId], [fChatId], [fSenderId], [fMessageText], [fSentAt], [fIsRead]) VALUES (13, 1, 1, N'好吧 那這金額我可以接受', CAST(N'2025-02-25T09:19:45.357' AS DateTime), 0)
GO
SET IDENTITY_INSERT [dbo].[tMessagesHistory] OFF
GO */
--　===== 聊天紀錄 END =====

--　===== 金流訂單 =====
-- ===== 綠界金流訂單資料表 =====
CREATE TABLE [dbo].[tEcpayOrders] (
    -- 訂單基本資訊
    [MerchantTradeNo]      NVARCHAR(50) NOT NULL,       -- 訂單編號（主鍵）：系統產生的唯一編號
    [MemberID]             NVARCHAR(50) NULL,           -- 會員編號：付款會員的識別碼
    [RtnCode]              INT NULL,                    -- 交易狀態代碼：1=成功，其他為失敗
    [RtnMsg]               NVARCHAR(50) NULL,           -- 交易訊息：如：交易成功、付款失敗等
    [TradeNo]              NVARCHAR(50) NULL,           -- 綠界交易編號：綠界系統的交易序號
    [TradeAmt]             INT NULL,                    -- 交易金額：訂單總金額
    [PaymentDate]          DATETIME NULL,               -- 付款時間：使用者實際付款的時間
    [PaymentType]          NVARCHAR(50) NULL,           -- 付款方式：如信用卡、ATM等
    [PaymentTypeChargeFee] NVARCHAR(50) NULL,           -- 交易手續費：支付給綠界的手續費
    [TradeDate]            NVARCHAR(50) NULL,           -- 訂單成立時間：訂單建立的時間
    [SimulatePaid]         INT NULL,                    -- 是否為模擬付款：0=實際付款，1=測試付款
    
    -- 關聯欄位（新增）
    [FTaskID]              INT NULL,                    -- 關聯任務ID：連結至任務資料表
    [FPosterID]            INT NULL,                    -- 發案者ID：付款方
    [FWorkerID]            INT NULL,                    -- 接案者ID：收款方
    
    -- 主鍵設定
    CONSTRAINT [PK_EcpayOrders] PRIMARY KEY CLUSTERED ([MerchantTradeNo] ASC),
    
    -- 外鍵約束（新增）
    CONSTRAINT [FK_EcpayOrders_Tasks] FOREIGN KEY ([FTaskID]) 
        REFERENCES [tTasks]([FTaskID]),
    CONSTRAINT [FK_EcpayOrders_Poster] FOREIGN KEY ([FPosterID]) 
        REFERENCES [tUsers]([FUserID]),
    CONSTRAINT [FK_EcpayOrders_Worker] FOREIGN KEY ([FWorkerID]) 
        REFERENCES [tUsers]([FUserID])
);

-- 插入測試資料
INSERT INTO [dbo].[tEcpayOrders] 
(
    [MerchantTradeNo], [MemberID], [RtnCode], [RtnMsg], [TradeNo], 
    [TradeAmt], [PaymentDate], [PaymentType], [PaymentTypeChargeFee], 
    [TradeDate], [SimulatePaid], [FTaskID], [FPosterID], [FWorkerID]
)
VALUES
-- 張小明(1)付款給李美玲(2)的訂單
(
    'TEST202403200001',                    -- 訂單編號
    's1237798s@gmail.com',				-- 會員編號（張小明的Email）
    10100058,                                -- 交易狀態（失敗）
    N'信用卡失敗',                         -- 交易訊息
    NULL,                                  -- 尚無綠界交易編號
    40000,                                 -- 交易金額
    NULL,                                  -- 尚未付款
    'Credit_CreditCard',                   -- 付款方式
    NULL,                                  -- 尚無手續費
    '2024-03-20 11:00:00',                -- 訂單成立時間
    0,                                     -- 實際付款
    5,                                     -- 任務ID
    1,                                     -- 發案者ID（張小明）
    2                                      -- 接案者ID（李美玲）
),

-- 張小明(1)付款給李美玲(2)的訂單
(
    'TEST202403200002',                    -- 訂單編號
    's1237798s@gmail.com',                  -- 會員編號（張小明的Email）
    10100058,                              -- 交易狀態（失敗）
    N'信用卡失敗',                         -- 交易訊息
    'ECPay20240320003',                    -- 綠界交易編號
    3000,                                 -- 交易金額
    '2024-03-20 14:45:00',                -- 付款時間
    'Credit_CreditCard',                   -- 付款方式
    '0',                                   -- 手續費
    '2024-03-20 14:45:00',                -- 訂單成立時間
    0,                                     -- 實際付款
    6,                                     -- 任務ID
    1,                                     -- 發案者ID（張小明）
    2                                      -- 接案者ID（李美玲）
),

-- 張小明(1)付款給李美玲(2)的訂單
(
    'TEST202403200004',                    -- 訂單編號
    's1237798s@gmail.com',                   -- 會員編號（大眾科技的Email）
    1,                                     -- 交易狀態（成功）
    N'交易成功',                           -- 交易訊息
    'ECPay20240320004',                    -- 綠界交易編號
    25000,                                  -- 交易金額
    '2024-03-20 16:20:00',                -- 付款時間
    'Credit_CreditCard',                   -- 付款方式
    '0',                                   -- 手續費
    '2024-03-20 16:20:00',                -- 訂單成立時間
    1,                                     -- 模擬付款
    7,                                     -- 任務ID
    1,                                     -- 發案者ID（張小明）
    2                                      -- 接案者ID（李美玲）
),

-- 張小明(1)付款給李美玲(2)的訂單
(
    'TEST202403200005',                    -- 訂單編號
    's1237798s@gmail.com',                -- 會員編號（陳小華的Email）
    1,                                     -- 交易狀態（成功）
    N'交易成功',                           -- 交易訊息
    'ECPay20240320005',                    -- 綠界交易編號
    50000,                                 -- 交易金額
    '2024-03-20 17:15:00',                -- 付款時間
    'ATM',                                 -- 付款方式
    '0',                                   -- 手續費
    '2024-03-20 17:15:00',                -- 訂單成立時間
    1,                                     -- 實際付款
    8,                                     -- 任務ID
    1,                                     -- 發案者ID（張小明）
    2                                      -- 接案者ID（李美玲）
);
--　===== 金流訂單 END =====

--　===== 用戶通知 =====
-- 通知類型
--CREATE TABLE tNotifyCategories(
--	fCategoryId INT PRIMARY KEY IDENTITY(1,1),
--	fCategoryName NVARCHAR(50) NOT NULL UNIQUE,
--);
-- 通知紀錄
CREATE TABLE tUserNotifications (
    fNotificationID INT PRIMARY KEY IDENTITY(1,1) ,
    fUserID INT NOT NULL, --接收通知 用戶ID
    fMessage NVARCHAR(MAX) NOT NULL,
    fRelatedID INT , --相關案號
    fSenderID INT , --發送通知 用戶ID 0表系統
    fNotifyType NVARCHAR(50) NOT NULL,
    fIsRead BIT NOT NULL DEFAULT 0,
    fCreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (fUserID) REFERENCES tUsers(FUserID),
    FOREIGN KEY (fSenderID) REFERENCES tUsers(FUserID),
	--FOREIGN KEY (fNotifyType) REFERENCES tNotifyCategories(fCategoryId) --目前不需要
);

--　===== 用戶通知 END =====

-- 發案者資料表：儲存發案者特定資訊 1/28新增關注欄位 --做程式要注意不能關注自己  
-- 02/14 停產 先暫時做worker 跟 task
--CREATE TABLE tFollows (
--    FFollowID INT IDENTITY(1,1) PRIMARY KEY,  -- 自動遞增的主鍵
--    FFollowerID INT NOT NULL,                 -- 追蹤者的 User ID
--    FFollowingID INT NOT NULL,                -- 被追蹤者的 User ID
--    FFollowerRole NVARCHAR(20) NOT NULL CHECK (FFollowerRole IN ('Worker', 'Poster')), -- 追蹤者的身份
--    FFollowingRole NVARCHAR(20) NOT NULL CHECK (FFollowingRole IN ('Worker', 'Poster')), -- 被追蹤者的身份
--    FCreatedAt DATETIME DEFAULT GETDATE(),    -- 追蹤時間

--    -- 設定外鍵，確保 Follower 和 Following 都來自 tUsers
--    CONSTRAINT FK_Follows_Follower FOREIGN KEY (FFollowerID) 
--        REFERENCES tUsers(FUserID) ON DELETE NO ACTION,   -- 改成 NO ACTION
--    CONSTRAINT FK_Follows_Following FOREIGN KEY (FFollowingID) 
--        REFERENCES tUsers(FUserID) ON DELETE NO ACTION,   -- 改成 NO ACTION

--    -- 防止用戶追蹤自己
--    CONSTRAINT CHK_Follower_Not_Same CHECK (FFollowerID <> FFollowingID),

--    -- 防止同一身份重複追蹤同一個人
--    CONSTRAINT UQ_Follows UNIQUE (FFollowerID, FFollowingID, FFollowerRole, FFollowingRole)
--);

--建立確認回覆表
CREATE TABLE tConfirmReplys(
	FConfirmReplyID INT IDENTITY(1,1) PRIMARY KEY,  -- 自動遞增的主鍵
	FTaskID INT NOT NULL,							-- 任務ID
	FPosterID INT NOT NULL,							-- 提案者ID
	FWorkerID INT NOT NULL,							-- 接案者ID
	FConfirmation_type NVARCHAR(20) NOT NULL,			-- 確認類型('接案確認', '提案確認','取消確認') 
	FConfirmation_status NVARCHAR(20) NOT NULL DEFAULT '待確認', -- 回覆狀態('待確認', '已確認', '已拒絕','已取消','拒絕取消')
	FRemarks TEXT,									-- 備註說明
	FOREIGN KEY (FTaskID) REFERENCES TTasks(FTaskID),
    FOREIGN KEY (FPosterID) REFERENCES tPosters(FUserID),
    FOREIGN KEY (FWorkerID) REFERENCES tWorkers(FUserID)
);

-- 建立交易紀錄表
CREATE TABLE TTransaction (
    TransactionID INT IDENTITY(1,1) PRIMARY KEY, -- 交易 ID，自動遞增主鍵
    Task_ID INT NOT NULL,                        -- 關聯的任務 ID
    PostUserID INT NOT NULL,                     -- 發案者 ID
    WorkUserID INT NOT NULL,                     -- 接案者 ID
    Amount INT NOT NULL,                         -- 交易金額
    Status NVARCHAR(50) NOT NULL,                -- 交易狀態 (進行中 / 已完成 / 已取消 /等待付款)
    StartTime DATETIME NOT NULL,                 -- 交易開始時間
    FinishTime DATETIME NULL,                    -- 交易完成時間
    Rating INT NULL,                             -- 發案者給接案者的評分 (1~5)
    Review NVARCHAR(MAX) NULL,                   -- 發案者對接案者的評論
    CONSTRAINT FK_Task FOREIGN KEY (Task_ID) REFERENCES TTasks(FtaskId),
    CONSTRAINT FK_PostUser FOREIGN KEY (PostUserID) REFERENCES TUsers(FuserId),
    CONSTRAINT FK_WorkUser FOREIGN KEY (WorkUserID) REFERENCES TUsers(FuserId)
);
SET IDENTITY_INSERT [dbo].[TTransaction] ON;
GO

INSERT INTO [dbo].[TTransaction] 
    ([TransactionID], [Task_ID], [PostUserID], [WorkUserID], [Amount], [Status], [StartTime], [FinishTime], [Rating], [Review])
VALUES
    -- 交易 1: 任務 5，由 1 號發案者委託 2 號接案者，狀態進行中
    (1, 5, 1, 2, 40000, N'進行中', '2025-02-24T16:41:41.737', '2025-03-10T12:30:00.000', null, null),
    -- 交易 2: 任務 6，由 1 號發案者委託 2 號接案者，狀態進行中
    (2, 6, 1, 2, 3000, N'進行中', '2025-02-24T16:41:41.737', '2025-03-10T12:30:00.000', null, null),
    -- 交易 3: 任務 7，由 1 號發案者委託 2 號接案者，金額 15000，狀態已完成
    (3, 7, 1, 2, 15000, N'已完成', '2025-02-24T16:41:41.737', '2025-03-01T15:00:00.000', 5, '下次還會找你'),
    -- 交易 4: 任務 8，由 1 號發案者委託 2 號接案者，狀態已完成
    (4, 8, 1, 2, 50000, N'已完成', '2025-02-24T16:41:41.737', '2025-02-10T12:30:00.000', 3, '時間有點久'),
	-- 交易 5: 任務 15，由 2 號發案者委託 1 號接案者，狀態進行中
    (5, 15, 2, 1, 14000, N'進行中', '2025-02-24T16:41:41.737', '2025-02-12T12:30:00.000', null, null),
	-- 交易 6: 任務 15，由 2 號發案者委託 1 號接案者，狀態進行中
    (6, 16, 2, 1, 7000, N'進行中', '2025-02-24T16:41:41.737', '2025-01-11T12:30:00.000', null, null),
	-- 交易 7: 任務 8，由 1 號發案者委託 2 號接案者，狀態已完成
    (7, 17, 2, 1, 10000, N'已完成', '2025-02-24T16:41:41.737', '2025-01-12T12:30:00.000', 4, '態度不錯'),
	-- 交易 7: 任務 8，由 1 號發案者委託 2 號接案者，狀態已完成
    (8, 18, 2, 1, 25000, N'已完成', '2025-02-24T16:41:41.737', '2025-03-07T12:30:00.000', 5, '效率好');	
    
GO
SET IDENTITY_INSERT [dbo].[TTransaction] OFF;
GO

