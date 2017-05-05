namespace Common3602
{
    /// <summary>
    /// WCF 相关
    /// </summary>
    public enum S3602Codes
    {
       
        ///<summary>
        /// CreateQuestionPage 操作
        ///</summary>
        MsgCreateQuestionsPage =1,

        ///<summary>
        /// ExamProduction 操作
        /// </summary>
        MsgExamProduction,

        ///<summary>
        /// EditPaperPage 操作
        /// </summary>
        MsgEditPaperPage,

        /// <summary>
        /// 获取题目的分类
        /// </summary>
        OptGetQuestionCategory,

        /// <summary>
        /// 新建分级
        /// </summary>
        OptCreateCategory,

        /// <summary>
        /// 删除分级
        /// </summary>
        OptDeleteCategory,

        ///<summary>
        /// 获取UMP安装路径的根目录
        /// </summary>
        OptGetUmpsetuppath,

        ///<summary>
        /// 添加题目
        /// </summary>
        OptAddQuestion,

        ///<summary>
        /// 获取题目
        /// </summary>
        OptGetQuestions,

        ///<summary>
        /// 新建题目
        /// </summary>
        OptCreateQuestion,

        ///<summary>
        /// 修改题目
        /// </summary>
        OptUpdateQuestion,

        ///<summary>
        /// 删除题目
        /// </summary>
        OptDeleteQuestion,

        ///<summary>
        /// 在同一个课程、课本、章节下试卷名不能相同
        /// </summary>
        OptPaperSameName,

        ///<summary>
        /// 获取试卷
        /// </summary>
        OptGetPapers,

        ///<summary>
        /// 获取试卷
        /// </summary>
        OptSearchPapers,

        /// <summary>
        /// 编辑试卷
        /// </summary>
        OptEditPaper,

        /// <summary>
        /// 创建试卷
        /// </summary>
        OptAddPaper,

        /// <summary>
        /// 修改试卷
        /// </summary>
        OptUpdatePaper,

        /// <summary>
        /// 删除试卷
        /// </summary>
        OptDeletePaper,

        /// <summary>
        /// 查看试卷
        /// </summary>
        OptCheckPaper,

        /// <summary>
        /// 获取试卷题目
        /// </summary>
        OptGetPaperQuestions,

        /// <summary>
        /// 判断试卷中题目是否存在
        /// </summary>
        OptPaperQuestionsExist,

        /// <summary>
        /// 添加试卷题目
        /// </summary>
        OptAddPaperQuestions,

        /// <summary>
        /// 修改试卷题目
        /// </summary>
        OptChangePaperQuestions,

        /// <summary>
        /// 删除试卷题目
        /// </summary>
        OptDeletePaperQuestions,

        /// <summary>
        /// 删除试卷所有题目
        /// </summary>
        OptDeletePaperAllQuestions,

        /// <summary>
        /// 查询问题
        /// </summary>
        OptQueryQuestions,

        /// <summary>
        /// 获取题所在的试卷ID
        /// </summary>
        OptGetQuestionsOfPaper,

        /// <summary>
        /// 设置题目数量
        /// </summary>
        OptSetQuestionNum,

         /// <summary>
        /// 加载文件Load
        /// </summary>
        OptLoadFile,

        ///<summary>
        /// 查询题目
        /// </summary>
        OptSearchQuestions,

        ///<summary>
        /// 查询试卷
        /// </summary>
        OptSearchPaper,

        /// <summary>
        /// 获取评分表影响的坐席
        /// </summary>
        OptGetCtrolAgent,

        /// <summary>
        /// 获取试卷考试人
        /// </summary>
        OptGetTestUserList,

        /// <summary>
        /// 设置试卷考试人
        /// </summary>
        OptSetTestUserList,

        
    }
}
