using Ndllm.Function.Prompt;
using Ndllm.Model;
using NextDesign.Desktop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Ndllm.Function.InteractivePrompt
{
    /// <summary>
    /// 対話型プロンプトを実行する
    /// </summary>
    internal class InteractivePrompt
    {
        private ICommandContext context;

        private string DefaultPrompt = "あなたはソフトウェア開発のスペシャリストです。\r\n次に与えるモデルに含まれる要求文に対し、要求仕様として不適切な個所を指摘してください。";

        public InteractivePrompt(ICommandContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// プロンプトを実行する
        /// </summary>
        internal void RunPrompt()
        {
            Task nowait = StartInteractivePromptAsync();
        }

        /// <summary>
        /// 対話型プロンプトダイアログを表示し、入力されたプロンプトを実行する
        /// </summary>
        /// <returns></returns>
        private async Task StartInteractivePromptAsync()
        {
            var dialog = new InteractivePromptDialog(DefaultPrompt);

            // イベントを購読して、プロンプトが準備できたら非同期処理を開始
            dialog.PromptReadyToExecute += async (sender, e) =>
            {
                // ダイアログから情報を取得して非同期処理を開始
                var inputPrompt = dialog.InputPrompt;
                var includeModelInfo = dialog.IncludeModelInfo;
                var includeChildModels = dialog.IncludeChildModels;

                if (String.IsNullOrEmpty(inputPrompt))
                {
                    return;
                }

                PromptModel promptModel = new PromptModel
                {
                    Connection = new ConnectionModel(),
                    Content = inputPrompt,
                    IncludeChildModels = includeChildModels,
                    TargetModels = new List<TargetModel>() // ここで必要なモデル情報を追加
                };

                // LLM接続情報とプロンプト実行
                PromptModelGenerator promptModelGenerator = new PromptModelGenerator(context.App);
                ConnectionModel connectionModel = promptModelGenerator.GetConnectionInfo();
                PromptRunner promptRunner = PromptRunner.Instance;
                await promptRunner.RunPromptAsync(context.App, promptModel, connectionModel, includeModelInfo);
            };

            dialog.Show(); 
        }
    }
}
