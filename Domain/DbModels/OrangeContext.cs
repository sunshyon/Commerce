using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Domain.DbModels
{
    public partial class OrangeContext : DbContext
    {
        public OrangeContext()
        {
        }

        public OrangeContext(DbContextOptions<OrangeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cid3> Cid3 { get; set; }
        public virtual DbSet<TbBrand> TbBrand { get; set; }
        public virtual DbSet<TbCategory> TbCategory { get; set; }
        public virtual DbSet<TbCategoryBrand> TbCategoryBrand { get; set; }
        public virtual DbSet<TbOrder> TbOrder { get; set; }
        public virtual DbSet<TbOrderDetail> TbOrderDetail { get; set; }
        public virtual DbSet<TbOrderStatus> TbOrderStatuse { get; set; }
        public virtual DbSet<TbPayLog> TbPayLog { get; set; }
        public virtual DbSet<TbSeckillOrder> TbSeckillOrder { get; set; }
        public virtual DbSet<TbSeckillSku> TbSeckillSku { get; set; }
        public virtual DbSet<TbSku> TbSku { get; set; }
        public virtual DbSet<TbSpecGroup> TbSpecGroup { get; set; }
        public virtual DbSet<TbSpecParam> TbSpecParam { get; set; }
        public virtual DbSet<TbSpu> TbSpu { get; set; }
        public virtual DbSet<TbSpuDetail> TbSpuDetail { get; set; }
        public virtual DbSet<TbStock> TbStock { get; set; }
        public virtual DbSet<TbUser> TbUser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=106.14.218.108;user id=root;password=123456;database=Orange", Microsoft.EntityFrameworkCore.ServerVersion.FromString("5.7.32-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cid3>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("cid3");

                entity.Property(e => e.ParentId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("parent_id")
                    .HasComment("父类目id,顶级类目填0");
            });

            modelBuilder.Entity<TbBrand>(entity =>
            {
                entity.ToTable("tb_brand");

                entity.HasComment("品牌表，一个品牌下有多个商品（spu），一对多关系");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id")
                    .HasComment("品牌id");

                entity.Property(e => e.Image)
                    .HasColumnType("varchar(128)")
                    .HasColumnName("image")
                    .HasDefaultValueSql("''")
                    .HasComment("品牌图片地址")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Letter)
                    .HasColumnType("char(1)")
                    .HasColumnName("letter")
                    .HasDefaultValueSql("''")
                    .HasComment("品牌的首字母")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("name")
                    .HasComment("品牌名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbCategory>(entity =>
            {
                entity.ToTable("tb_category");

                entity.HasComment("商品类目表，类目和商品(spu)是一对多关系，类目与品牌是多对多关系");

                entity.HasIndex(e => e.ParentId, "key_parent_id");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id")
                    .HasComment("类目id");

                entity.Property(e => e.IsParent)
                    .HasColumnName("is_parent")
                    .HasComment("是否为父节点，0为否，1为是");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("name")
                    .HasComment("类目名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("parent_id")
                    .HasComment("父类目id,顶级类目填0");

                entity.Property(e => e.Sort)
                    .HasColumnType("int(4)")
                    .HasColumnName("sort")
                    .HasComment("排序指数，越小越靠前");
            });

            modelBuilder.Entity<TbCategoryBrand>(entity =>
            {
                entity.HasKey(e => new { e.CategoryId, e.BrandId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("tb_category_brand");

                entity.HasComment("商品分类和品牌的中间表，两者是多对多关系");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("category_id")
                    .HasComment("商品类目id");

                entity.Property(e => e.BrandId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("brand_id")
                    .HasComment("品牌id");
            });

            modelBuilder.Entity<TbOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("tb_order");

                entity.HasIndex(e => e.BuyerNick, "buyer_nick");

                entity.HasIndex(e => e.CreateTime, "create_time");

                entity.Property(e => e.OrderId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever()
                    .HasColumnName("order_id")
                    .HasComment("订单id");

                entity.Property(e => e.ActualPay)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("actual_pay")
                    .HasComment("实付金额。单位:分。如:20007，表示:200元7分");

                entity.Property(e => e.BuyerMessage)
                    .HasColumnType("varchar(128)")
                    .HasColumnName("buyer_message")
                    .HasComment("买家留言")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.BuyerNick)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("buyer_nick")
                    .HasComment("买家昵称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.BuyerRate)
                    .HasColumnName("buyer_rate")
                    .HasComment("买家是否已经评价,0未评价，1已评价");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("订单创建时间");

                entity.Property(e => e.InvoiceType)
                    .HasColumnType("int(1)")
                    .HasColumnName("invoice_type")
                    .HasDefaultValueSql("'0'")
                    .HasComment("发票类型(0无发票1普通发票，2电子发票，3增值税发票)");

                entity.Property(e => e.PaymentType)
                    .HasColumnType("tinyint(1) unsigned zerofill")
                    .HasColumnName("payment_type")
                    .HasComment("支付类型，1、在线支付，2、货到付款");

                entity.Property(e => e.PostFee)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("post_fee")
                    .HasComment("邮费。单位:分。如:20007，表示:200元7分");

                entity.Property(e => e.PromotionIds)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("promotion_ids")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.Receiver)
                    .HasColumnType("varchar(32)")
                    .HasColumnName("receiver")
                    .HasComment("收货人")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.ReceiverAddress)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("receiver_address")
                    .HasDefaultValueSql("''")
                    .HasComment("收获地址（街道、住址等详细地址）")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.ReceiverCity)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("receiver_city")
                    .HasDefaultValueSql("''")
                    .HasComment("收获地址（市）")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.ReceiverDistrict)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("receiver_district")
                    .HasDefaultValueSql("''")
                    .HasComment("收获地址（区/县）")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.ReceiverMobile)
                    .HasColumnType("varchar(11)")
                    .HasColumnName("receiver_mobile")
                    .HasComment("收货人手机")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.ReceiverState)
                    .HasColumnType("varchar(128)")
                    .HasColumnName("receiver_state")
                    .HasDefaultValueSql("''")
                    .HasComment("收获地址（省）")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.ReceiverZip)
                    .HasColumnType("varchar(16)")
                    .HasColumnName("receiver_zip")
                    .HasComment("收货人邮编")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.ShippingCode)
                    .HasColumnType("varchar(20)")
                    .HasColumnName("shipping_code")
                    .HasComment("物流单号")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.ShippingName)
                    .HasColumnType("varchar(20)")
                    .HasColumnName("shipping_name")
                    .HasComment("物流名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.SourceType)
                    .HasColumnType("int(1)")
                    .HasColumnName("source_type")
                    .HasDefaultValueSql("'2'")
                    .HasComment("订单来源：1:app端，2：pc端，3：M端，4：微信端，5：手机qq端");

                entity.Property(e => e.TotalPay)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("total_pay")
                    .HasComment("总金额，单位为分");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("user_id")
                    .HasComment("用户id")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");
            });

            modelBuilder.Entity<TbOrderDetail>(entity =>
            {
                entity.ToTable("tb_order_detail");

                entity.HasComment("订单详情表");

                entity.HasIndex(e => e.OrderId, "key_order_id");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id")
                    .HasComment("订单详情id ");

                entity.Property(e => e.Image)
                    .HasColumnType("varchar(128)")
                    .HasColumnName("image")
                    .HasDefaultValueSql("''")
                    .HasComment("商品图片")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Num)
                    .HasColumnType("int(11)")
                    .HasColumnName("num")
                    .HasComment("购买数量");

                entity.Property(e => e.OrderId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("order_id")
                    .HasComment("订单id");

                entity.Property(e => e.OwnSpec)
                    .HasColumnType("varchar(1024)")
                    .HasColumnName("own_spec")
                    .HasDefaultValueSql("''")
                    .HasComment("商品动态属性键值集")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("price")
                    .HasComment("价格,单位：分");

                entity.Property(e => e.SkuId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("sku_id")
                    .HasComment("sku商品id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("varchar(256)")
                    .HasColumnName("title")
                    .HasComment("商品标题")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbOrderStatus>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("tb_order_status");

                entity.HasComment("订单状态表");

                entity.HasIndex(e => e.Status, "status");

                entity.Property(e => e.OrderId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever()
                    .HasColumnName("order_id")
                    .HasComment("订单id");

                entity.Property(e => e.CloseTime)
                    .HasColumnType("datetime")
                    .HasColumnName("close_time")
                    .HasComment("交易关闭时间");

                entity.Property(e => e.CommentTime)
                    .HasColumnType("datetime")
                    .HasColumnName("comment_time")
                    .HasComment("评价时间");

                entity.Property(e => e.ConsignTime)
                    .HasColumnType("datetime")
                    .HasColumnName("consign_time")
                    .HasComment("发货时间");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("订单创建时间");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_time")
                    .HasComment("交易完成时间");

                entity.Property(e => e.PaymentTime)
                    .HasColumnType("datetime")
                    .HasColumnName("payment_time")
                    .HasComment("付款时间");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasColumnName("status")
                    .HasComment("状态：1、未付款 2、已付款,未发货 3、已发货,未确认 4、交易成功 5、交易关闭 6、已评价");
            });

            modelBuilder.Entity<TbPayLog>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("tb_pay_log");

                entity.Property(e => e.OrderId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever()
                    .HasColumnName("order_id")
                    .HasComment("订单号");

                entity.Property(e => e.BankType)
                    .HasColumnType("varchar(16)")
                    .HasColumnName("bank_type")
                    .HasComment("银行类型")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ClosedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("closed_time")
                    .HasComment("关闭时间");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("创建时间");

                entity.Property(e => e.PayTime)
                    .HasColumnType("datetime")
                    .HasColumnName("pay_time")
                    .HasComment("支付时间");

                entity.Property(e => e.PayType)
                    .HasColumnName("pay_type")
                    .HasComment("支付方式，1 微信支付, 2 货到付款");

                entity.Property(e => e.RefundTime)
                    .HasColumnType("datetime")
                    .HasColumnName("refund_time")
                    .HasComment("退款时间");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("交易状态，1 未支付, 2已支付, 3 已退款, 4 支付错误, 5 已关闭");

                entity.Property(e => e.TotalFee)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("total_fee")
                    .HasComment("支付金额（分）");

                entity.Property(e => e.TransactionId)
                    .HasColumnType("varchar(32)")
                    .HasColumnName("transaction_id")
                    .HasComment("微信交易号码")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UserId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("user_id")
                    .HasComment("用户ID");
            });

            modelBuilder.Entity<TbSeckillOrder>(entity =>
            {
                entity.ToTable("tb_seckill_order");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id")
                    .HasComment("秒杀订单标识");

                entity.Property(e => e.OrderId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("order_id")
                    .HasComment("秒杀订单号");

                entity.Property(e => e.SkuId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("sku_id")
                    .HasComment("秒杀商品ID");

                entity.Property(e => e.UserId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("user_id")
                    .HasComment("用户编号");
            });

            modelBuilder.Entity<TbSeckillSku>(entity =>
            {
                entity.ToTable("tb_seckill_sku");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id")
                    .HasComment("秒杀ID");

                entity.Property(e => e.Enable)
                    .HasColumnName("enable")
                    .HasComment("是否允许秒杀 1-允许，0-不允许");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_time")
                    .HasComment("秒杀结束时间");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnType("varchar(256)")
                    .HasColumnName("image")
                    .HasComment("秒杀商品图片")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SeckillPrice)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("seckill_price")
                    .HasComment("秒杀价格");

                entity.Property(e => e.SkuId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("sku_id")
                    .HasComment("秒杀的商品skuId");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_time")
                    .HasComment("秒杀开始时间");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("varchar(256)")
                    .HasColumnName("title")
                    .HasComment("秒杀商品标题")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbSku>(entity =>
            {
                entity.ToTable("tb_sku");

                entity.HasComment("sku表,该表表示具体的商品实体,如黑色的 64g的iphone 8");

                entity.HasIndex(e => e.SpuId, "key_spu_id");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id")
                    .HasComment("sku id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("添加时间");

                entity.Property(e => e.Enable)
                    .IsRequired()
                    .HasColumnName("enable")
                    .HasDefaultValueSql("'1'")
                    .HasComment("是否有效，0无效，1有效");

                entity.Property(e => e.Images)
                    .HasColumnType("varchar(1024)")
                    .HasColumnName("images")
                    .HasDefaultValueSql("''")
                    .HasComment("商品的图片，多个图片以‘,’分割")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Indexes)
                    .HasColumnType("varchar(32)")
                    .HasColumnName("indexes")
                    .HasDefaultValueSql("''")
                    .HasComment("特有规格属性在spu属性模板中的对应下标组合")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastUpdateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("last_update_time")
                    .HasComment("最后修改时间");

                entity.Property(e => e.OwnSpec)
                    .HasColumnType("varchar(1024)")
                    .HasColumnName("own_spec")
                    .HasDefaultValueSql("''")
                    .HasComment("sku的特有规格参数键值对，json格式，反序列化时请使用linkedHashMap，保证有序")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price)
                    .HasColumnType("bigint(15)")
                    .HasColumnName("price")
                    .HasComment("销售价格，单位为分");

                entity.Property(e => e.SpuId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("spu_id")
                    .HasComment("spu id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("varchar(256)")
                    .HasColumnName("title")
                    .HasComment("商品标题")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbSpecGroup>(entity =>
            {
                entity.ToTable("tb_spec_group");

                entity.HasComment("规格参数的分组表，每个商品分类下有多个规格参数组");

                entity.HasIndex(e => e.Cid, "key_category");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id")
                    .HasComment("主键");

                entity.Property(e => e.Cid)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("cid")
                    .HasComment("商品分类id，一个分类下有多个规格组");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("name")
                    .HasComment("规格组的名称")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbSpecParam>(entity =>
            {
                entity.ToTable("tb_spec_param");

                entity.HasComment("规格参数组下的参数名");

                entity.HasIndex(e => e.Cid, "key_category");

                entity.HasIndex(e => e.GroupId, "key_group");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id")
                    .HasComment("主键");

                entity.Property(e => e.Cid)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("cid")
                    .HasComment("商品分类id");

                entity.Property(e => e.Generic)
                    .HasColumnName("generic")
                    .HasComment("是否是sku通用属性，true或false");

                entity.Property(e => e.GroupId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("group_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(256)")
                    .HasColumnName("name")
                    .HasComment("参数名")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Numeric)
                    .HasColumnName("numeric")
                    .HasComment("是否是数字类型参数，true或false");

                entity.Property(e => e.Searching)
                    .HasColumnName("searching")
                    .HasComment("是否用于搜索过滤，true或false");

                entity.Property(e => e.Segments)
                    .HasColumnType("varchar(1024)")
                    .HasColumnName("segments")
                    .HasDefaultValueSql("''")
                    .HasComment("数值类型参数，如果需要搜索，则添加分段间隔值，如CPU频率间隔：0.5-1.0")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Unit)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("unit")
                    .HasDefaultValueSql("''")
                    .HasComment("数字类型参数的单位，非数字类型可以为空")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbSpu>(entity =>
            {
                entity.ToTable("tb_spu");

                entity.HasComment("spu表，该表描述的是一个抽象性的商品，比如 iphone8");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id")
                    .HasComment("spu id");

                entity.Property(e => e.BrandId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("brand_id")
                    .HasComment("商品所属品牌id");

                entity.Property(e => e.Cid1)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("cid1")
                    .HasComment("1级类目id");

                entity.Property(e => e.Cid2)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("cid2")
                    .HasComment("2级类目id");

                entity.Property(e => e.Cid3)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("cid3")
                    .HasComment("3级类目id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_time")
                    .HasComment("添加时间");

                entity.Property(e => e.LastUpdateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("last_update_time")
                    .HasComment("最后修改时间");

                entity.Property(e => e.Saleable)
                    .IsRequired()
                    .HasColumnName("saleable")
                    .HasDefaultValueSql("'1'")
                    .HasComment("是否上架，0下架，1上架");

                entity.Property(e => e.SubTitle)
                    .HasColumnType("varchar(256)")
                    .HasColumnName("sub_title")
                    .HasDefaultValueSql("''")
                    .HasComment("子标题")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("varchar(128)")
                    .HasColumnName("title")
                    .HasDefaultValueSql("''")
                    .HasComment("标题")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Valid)
                    .IsRequired()
                    .HasColumnName("valid")
                    .HasDefaultValueSql("'1'")
                    .HasComment("是否有效，0已删除，1有效");
            });

            modelBuilder.Entity<TbSpuDetail>(entity =>
            {
                entity.HasKey(e => e.SpuId)
                    .HasName("PRIMARY");

                entity.ToTable("tb_spu_detail");

                entity.Property(e => e.SpuId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever()
                    .HasColumnName("spu_id");

                entity.Property(e => e.AfterService)
                    .HasColumnType("varchar(1024)")
                    .HasColumnName("after_service")
                    .HasDefaultValueSql("''")
                    .HasComment("售后服务")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description")
                    .HasComment("商品描述信息")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.GenericSpec)
                    .IsRequired()
                    .HasColumnType("varchar(2048)")
                    .HasColumnName("generic_spec")
                    .HasDefaultValueSql("''")
                    .HasComment("通用规格参数数据")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PackingList)
                    .HasColumnType("varchar(1024)")
                    .HasColumnName("packing_list")
                    .HasDefaultValueSql("''")
                    .HasComment("包装清单")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SpecialSpec)
                    .IsRequired()
                    .HasColumnType("varchar(1024)")
                    .HasColumnName("special_spec")
                    .HasComment("特有规格参数及可选值信息，json格式")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TbStock>(entity =>
            {
                entity.HasKey(e => e.SkuId)
                    .HasName("PRIMARY");

                entity.ToTable("tb_stock");

                entity.HasComment("库存表，代表库存，秒杀库存等信息");

                entity.Property(e => e.SkuId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever()
                    .HasColumnName("sku_id")
                    .HasComment("库存对应的商品sku id");

                entity.Property(e => e.SeckillStock)
                    .HasColumnType("int(9)")
                    .HasColumnName("seckill_stock")
                    .HasDefaultValueSql("'0'")
                    .HasComment("可秒杀库存");

                entity.Property(e => e.SeckillTotal)
                    .HasColumnType("int(9)")
                    .HasColumnName("seckill_total")
                    .HasDefaultValueSql("'0'")
                    .HasComment("秒杀总数量");

                entity.Property(e => e.Stock)
                    .HasColumnType("int(9)")
                    .HasColumnName("stock")
                    .HasComment("库存数量");
            });

            modelBuilder.Entity<TbUser>(entity =>
            {
                entity.ToTable("tb_user");

                entity.HasComment("用户表");

                entity.HasIndex(e => e.Username, "username")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created")
                    .HasComment("创建时间");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("password")
                    .HasComment("密码，加密存储")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Phone)
                    .HasColumnType("varchar(11)")
                    .HasColumnName("phone")
                    .HasComment("注册手机号")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("salt")
                    .HasComment("密码加密的salt值")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("varchar(32)")
                    .HasColumnName("username")
                    .HasComment("用户名")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
