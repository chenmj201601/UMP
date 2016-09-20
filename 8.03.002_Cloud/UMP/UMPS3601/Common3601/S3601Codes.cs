namespace Common3601
{
    /// <summary>
    /// WCF 相关
    /// </summary>
    public enum S3601Codes
    {
       
        ///<summary>
        /// CreateQuestionPage 操作
        ///</summary>
        MessageCreatequestionspage =1,

        ///<summary>
        /// ExamProduction 操作
        /// </summary>
        MessageExamproduction,

        ///<summary>
        /// EditPaperPage 操作
        /// </summary>
        MessageEditPaperPage,

        ///<summary>
        /// CreateCategory 操作
        ///</summary>
        MessageCreateCategory,

        ///<summary>
        /// UpdateCategory 操作
        /// </summary>
        MessageUpdateCategory,

        /// <summary>
        /// 获取题目的分类
        /// </summary>
        OperationGetQuestionCategory,

        /// <summary>
        /// 新建分级
        /// </summary>
        OperationCreateCategory,

        /// <summary>
        /// 修改分级
        /// </summary>
        OperationUpdateCategory,

        /// <summary>
        /// 删除分级
        /// </summary>
        OperationDeleteCategory,

        ///<summary>
        /// 获取UMP安装路径的根目录
        /// </summary>
        OperationGetUmpsetuppath,

        ///<summary>
        /// 添加题目
        /// </summary>
        OperationAddQuestion,

        ///<summary>
        /// 获取题目
        /// </summary>
        OperationGetQuestions,

        ///<summary>
        /// 查询题目
        /// </summary>
        OperationSearchQuestions,

        ///<summary>
        /// 新建题目
        /// </summary>
        OperationCreateQuestion,

        ///<summary>
        /// 修改题目
        /// </summary>
        OperationUpdateQuestion,

        ///<summary>
        /// 删除题目
        /// </summary>
        OperationDeleteQuestion,

        ///<summary>
        /// 在同一个课程、课本、章节下试卷名不能相同
        /// </summary>
        OperationPaperSameName,

        ///<summary>
        /// 获取试卷
        /// </summary>
        OperationGetPapers,

        ///<summary>
        /// 获取试卷
        /// </summary>
        OperationSearchPapers,

        /// <summary>
        /// 编辑试卷
        /// </summary>
        OperationEditPaper,

        /// <summary>
        /// 创建试卷
        /// </summary>
        OperationAddPaper,

        /// <summary>
        /// 修改试卷
        /// </summary>
        OperationUpdatePaper,

        /// <summary>
        /// 删除试卷
        /// </summary>
        OperationDeletePaper,

        /// <summary>
        /// 查看试卷
        /// </summary>
        OperationCheckPaper,

        /// <summary>
        /// 获取试卷题目
        /// </summary>
        OperationGetPaperQuestions,

        /// <summary>
        /// 判断试卷中题目是否存在
        /// </summary>
        OperationPaperQuestionsExist,

        /// <summary>
        /// 添加试卷题目
        /// </summary>
        OperationAddPaperQuestions,

        /// <summary>
        /// 删除试卷题目
        /// </summary>
        OperationDeletePaperQuestions,

        /// <summary>
        /// 删除试卷所有题目
        /// </summary>
        OperationDeletePaperAllQuestions,

        /// <summary>
        /// 查询问题
        /// </summary>
        OperationQueryQuestions,

        /// <summary>
        /// 获取题所在的试卷ID
        /// </summary>
        OperationGetQuestionsOfPaper,

        /// <summary>
        /// 加载文件Load
        /// </summary>
        OperationLoadFile,

        /// <summary>
        /// 获取上传文件路径
        /// </summary>
        OperationGetUploadFilePath,

        /// <summary>
        /// 导入文件
        /// </summary>
        OperationImportExcelFile,
    }
}
