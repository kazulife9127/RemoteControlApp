using Azure.AI.OpenAI;
using Azure.Core.Pipeline;
using Azure;
using NextDesign.Core;
using NextDesign.Desktop;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Ndllm.Model;
using Ndllm.Function.Output;
using System.Collections;
using System.Linq;

namespace Ndllm.Function.Prompt
{
    /// <summary>
    /// プロンプトを実行する
    /// </summary>
    internal class PromptRunner
    {

        private List<string> ModelInfo;
        private string DeploymentName;

        private ConversationHistoryManager historyManager = ConversationHistoryManager.Instance;

        private static PromptRunner instance;        

        private PromptRunner() { }

        public static PromptRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PromptRunner();
                }
                return instance;
            }
        }

        /// <summary>
        /// モデル情報とLLM接続情報を取得し、プロンプトを実行する
        /// </summary>
        /// <param name="app"></param>
        /// <param name="inputPrompt"></param>
        /// <returns></returns>

        // 対話プロンプト
        internal async Task RunPromptAsync(IApplication app, PromptModel promptModel, ConnectionModel connectionModel, bool includeModelInfo)
        {
            // 会話履歴をトリミング
            historyManager.TrimConversationHistory();

            // 配下のモデルを含むか
            var includeChildModels = promptModel.IncludeChildModels;
            var targetModels = promptModel.TargetModels;

            // モデル情報を取得
            ModelInfo = GetModelInfo(app, includeChildModels, targetModels);
            // モデル情報のチェックがTrueだったら
            // モデル情報を1行にする(改行を含む文字列、表示は1列)
            string checkedModelInfoString = includeModelInfo ? GetModelInfoMessage(ModelInfo) : string.Empty;

            List<string> checkedModelInfoList = new List<string>();

            // String型をList型に変更
            if (!String.IsNullOrEmpty(checkedModelInfoString))
            {
                checkedModelInfoList = new List<string>(checkedModelInfoString.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
            }

            // プロンプトに会話履歴を追加
            string history = historyManager.GetHistoryAsString();
            string prompt = "#指示内容" + Environment.NewLine + Environment.NewLine + promptModel.Content + Environment.NewLine + history;

            if (!String.IsNullOrEmpty(checkedModelInfoString)) {
                prompt += checkedModelInfoString;
            }

            // LLM接続情報を取得
            ConnectionModel connection = promptModel.Connection;
            if (connection.ModelType == null)
            {
                // LlmSetting.txtの接続情報
                connection = connectionModel;
            }

            OpenAIClient client = GetOpenAIClient(connection);

            try
            {
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = DeploymentName, // Use DeploymentName for "model" with non-Azure clients
                    Messages =
                    {
                        new ChatRequestUserMessage(prompt),
                    }
                };

                Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
                ChatResponseMessage responseMessage = response.Value.Choices[0].Message;

                historyManager.AddToHistory(prompt);
                historyManager.AddToHistory(responseMessage.Content);

                // メッセージ出力
                MessageOutputManager.OutputMessage(app, promptModel.Content, checkedModelInfoList, responseMessage.Content);
            }
            catch (Exception ex)
            {
                // エラー出力
                Output.Output.OutputErrorMessage(app, ex.ToString());
            }
        }
    }
}