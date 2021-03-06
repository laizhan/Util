﻿using System;
using Util.Properties;
using Util.Datas.Sql.Queries.Builders.Abstractions;
using Util.Datas.Sql.Queries.Builders.Core;
using Util.Datas.Sql.Queries.Builders.Extensions;

namespace Util.Datas.Sql.Queries.Builders.Clauses {
    /// <summary>
    /// From子句
    /// </summary>
    public class FromClause : IFromClause {
        /// <summary>
        /// 方言
        /// </summary>
        protected readonly IDialect Dialect;
        /// <summary>
        /// 实体解析器
        /// </summary>
        protected readonly IEntityResolver Resolver;
        /// <summary>
        /// 实体注册器
        /// </summary>
        protected readonly IEntityAliasRegister Register;
        /// <summary>
        /// Sql项
        /// </summary>
        protected SqlItem Table;

        /// <summary>
        /// 初始化From子句
        /// </summary>
        /// <param name="dialect">方言</param>
        /// <param name="resolver">实体解析器</param>
        /// <param name="register">实体别名注册器</param>
        /// <param name="table">表</param>
        public FromClause( IDialect dialect, IEntityResolver resolver, IEntityAliasRegister register, SqlItem table = null ) {
            Dialect = dialect;
            Resolver = resolver;
            Register = register;
            Table = table;
        }

        /// <summary>
        /// 复制From子句
        /// </summary>
        /// <param name="register">实体别名注册器</param>
        public virtual IFromClause Clone( IEntityAliasRegister register ) {
            return new FromClause( Dialect, Resolver, register, Table );
        }

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        public void From( string table, string alias = null ) {
            Table = CreateSqlItem( table, null, alias );
        }

        /// <summary>
        /// 创建Sql项
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="schema">架构名</param>
        /// <param name="alias">别名</param>
        protected virtual SqlItem CreateSqlItem( string table, string schema, string alias ) {
            return new SqlItem( table, schema, alias );
        }

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="schema">架构名</param>
        public void From<TEntity>( string alias = null, string schema = null ) where TEntity : class {
            var entity = typeof( TEntity );
            var table = Resolver.GetTableAndSchema( entity );
            Table = CreateSqlItem( table, schema, alias );
            Register.Register( entity, Resolver.GetAlias( entity, alias ) );
        }

        /// <summary>
        /// 添加到From子句
        /// </summary>
        /// <param name="sql">Sql语句</param>
        public void AppendSql( string sql ) {
            if( Table != null && Table.Raw ) {
                Table = new SqlItem( $"{Table.Name}{sql}", raw: true );
                return;
            }
            Table = new SqlItem( sql, raw: true );
        }

        /// <summary>
        /// 验证
        /// </summary>
        public void Validate() {
            if( string.IsNullOrWhiteSpace( Table?.Name ) )
                throw new InvalidOperationException( LibraryResource.TableIsEmpty );
        }

        /// <summary>
        /// 输出Sql
        /// </summary>
        public string ToSql() {
            var table = Table?.ToSql( Dialect );
            if( string.IsNullOrWhiteSpace( table ) )
                return null;
            return $"From {table}";
        }
    }
}
