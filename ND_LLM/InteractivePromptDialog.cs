using System;
using System.Windows.Forms;

namespace Ndllm.Function.InteractivePrompt
{
    public partial class InteractivePromptDialog : Form
    {
        private string inputPrompt;
        private bool includeModelInfo;
        private bool includeChildModels;

        public event EventHandler PromptReadyToExecute;


        public InteractivePromptDialog(string defaultPrompt)
        {
            InitializeComponent();

            this.PromptText.Text = defaultPrompt;
        }

        internal string InputPrompt
        {
            set { this.inputPrompt = value; }
            get { return this.inputPrompt; }
        }

        internal bool IncludeModelInfo
        {
            set { this.includeModelInfo = value; }
            get { return this.includeModelInfo; }
        }

        internal bool IncludeChildModels
        {
            set { this.includeChildModels = value; }
            get { return this.includeChildModels; }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            this.InputPrompt = this.PromptText.Text;
            this.IncludeModelInfo = ModelInfoCheckBox.Checked;
            this.IncludeChildModels = ChildModelCheckBox.Checked;

            // プロンプト実行の準備ができたことを通知するイベントを発火
            PromptReadyToExecute?.Invoke(this, EventArgs.Empty);

            // テキストボックスの内容を空にする
            this.PromptText.Text = String.Empty;

            // モデル情報を含めるチェックボックスをOFFにする
            this.ModelInfoCheckBox.Checked = false;
        }
    }
}
