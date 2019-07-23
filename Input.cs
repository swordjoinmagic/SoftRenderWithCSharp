using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortRenderWithCSharp {
    public class Input {

        public static Input Instance = new Input();

        // 表示鼠标是否按下
        public bool isMouseDown;
        // 表示是否有键盘按下
        public bool isKeyDown;
        // 表示当前按下的鼠标的键位
        public MouseButtons mouseCode;
        // 表示当前按下的键盘的键位
        public Keys keyCode;

        public void Reset() {
            isMouseDown = false;
            isKeyDown = false;
            mouseCode = MouseButtons.None;
            keyCode = Keys.None;
        }

        public void OperateKeybordEvent(KeyEventArgs keyEvent) {
            isKeyDown = true;
            keyCode = keyEvent.KeyCode;
        }

        public void OperateMouseEvent(MouseEventArgs mouseEvent) {
            isMouseDown = true;
            mouseCode = mouseEvent.Button;
            
        }
    }
}
