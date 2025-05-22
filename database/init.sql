-- Supabase数据库初始化脚本
-- 创建todos表用于ASP.NET Core Todo List应用

-- 创建todos表
CREATE TABLE IF NOT EXISTS todos (
    id SERIAL PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    description VARCHAR(500),
    is_completed BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 添加表注释
COMMENT ON TABLE todos IS 'Todo任务表';
COMMENT ON COLUMN todos.id IS '任务ID，主键';
COMMENT ON COLUMN todos.title IS '任务标题，最大200字符';
COMMENT ON COLUMN todos.description IS '任务描述，最大500字符，可为空';
COMMENT ON COLUMN todos.is_completed IS '任务完成状态，默认false';
COMMENT ON COLUMN todos.created_at IS '创建时间';
COMMENT ON COLUMN todos.updated_at IS '更新时间';

-- 创建索引以提高查询性能
CREATE INDEX IF NOT EXISTS idx_todos_created_at ON todos(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_todos_is_completed ON todos(is_completed);
CREATE INDEX IF NOT EXISTS idx_todos_title ON todos USING gin(to_tsvector('english', title));

-- 创建触发器函数以自动更新updated_at字段
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- 创建触发器
DROP TRIGGER IF EXISTS update_todos_updated_at ON todos;
CREATE TRIGGER update_todos_updated_at
    BEFORE UPDATE ON todos
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- 插入示例数据（可选）
INSERT INTO todos (title, description, is_completed) VALUES
('学习ASP.NET Core', '完成ASP.NET Core基础教程学习', false),
('设置Supabase数据库', '配置Supabase项目并创建数据库表', true),
('开发Todo API', '实现完整的CRUD操作API', false),
('设计前端界面', '创建响应式的用户界面', false),
('编写项目文档', '完善README和API文档', false)
ON CONFLICT DO NOTHING;

-- 验证表创建
SELECT 
    table_name, 
    column_name, 
    data_type, 
    is_nullable,
    column_default
FROM information_schema.columns 
WHERE table_name = 'todos' 
ORDER BY ordinal_position;

-- 显示表结构信息
\d+ todos;
